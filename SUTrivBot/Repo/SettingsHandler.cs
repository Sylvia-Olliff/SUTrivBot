using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using NLog;
using SUTrivBot.Lib;
using SUTrivBot.Models;

namespace SUTrivBot.Repo
{
    public static class SettingsHandler
    {
        private static readonly SqliteConnectionStringBuilder ConnectionStringBuilder = new SqliteConnectionStringBuilder();
        private static readonly Dictionary<ulong, GuildSettings> Cache = new Dictionary<ulong, GuildSettings>();
        private static readonly ILogger Logger;
        
        private const string TableNameGuild = "GuildSettings";
        private const string TableNameChannel = "Channels";

        static SettingsHandler()
        {
            ConnectionStringBuilder.DataSource = "peribot.db";
            Logger = ConfigBuilder.Build().Logger;
            Init().Wait();
        }

        private static async Task Init()
        {
            try
            {
                await using var connection = new SqliteConnection(ConnectionStringBuilder.ConnectionString);
                await connection.OpenAsync();
                
                // TODO: add an appsettings configuration to trigger table drop. 

                var checkGuildTableExists = connection.CreateCommand();
                checkGuildTableExists.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{TableNameGuild}'";
                await using var guildTableReader = await checkGuildTableExists.ExecuteReaderAsync();
                if (!guildTableReader.HasRows)
                {
                    await CreateGuildTable(connection);
                }
                
                var checkChannelTableExists = connection.CreateCommand();
                checkChannelTableExists.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{TableNameChannel}'";
                await using var channelTableReader = await checkChannelTableExists.ExecuteReaderAsync();
                if (!channelTableReader.HasRows)
                {
                    await CreateChannelTable(connection);
                }

                await connection.CloseAsync();
                Logger.Info("SQLite DB successfully created.");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error Initializing SQLite DB");
            }
        }

        private static async Task CreateGuildTable(SqliteConnection connection)
        {
            try
            {
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = $"CREATE TABLE {TableNameGuild}(" +
                                                 "GuildID UNSIGNED BIG INT PRIMARY KEY NOT NULL UNIQUE," +
                                                 "GuildName VARCHAR(50) NOT NULL," +
                                                 "Disabled BOOLEAN DEFAULT(0)," +
                                                 "RestrictTriviaMaster BOOLEAN DEFAULT(1))";
                await createTableCommand.ExecuteNonQueryAsync();
                Logger.Info("Guild Table successfully created!");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to create Guild Table!!!");
            }
        }

        private static async Task CreateChannelTable(SqliteConnection connection)
        {
            try
            {
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = $"CREATE TABLE {TableNameChannel}(" +
                                                 "ChannelID UNSIGNED BIG INT PRIMARY KEY NOT NULL UNIQUE," +
                                                 "GuildID UNSIGNED BIG INT NOT NULL," +
                                                 "ChannelName VARCHAR(50) NOT NULL," +
                                                 $"FOREIGN KEY(GuildID) REFERENCES {TableNameGuild}(GuildID))";
                await createTableCommand.ExecuteNonQueryAsync();
                Logger.Info("Channel Table successfully created!");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to create Channel Table!!!");
            }
        }

        private static async Task<bool> UpdateChannels(GuildSettings settings, SqliteConnection connection)
        {
            try
            {
                // For the sake of simplicity with Channels, all referenced channels will be deleted and rebuilt each time
                var commands = new List<string> {$"DELETE FROM {TableNameChannel} WHERE GuildID = {settings.GuildId}"};
                
                commands.AddRange(settings.LockedChannels.Select(item => $"INSERT INTO {TableNameChannel} " +
                                                                         $"(ChannelID, GuildID, ChannelName) " + 
                                                                         $"VALUES ({item.ChannelId}, {item.GuildId}, '{item.ChannelName}'"));

                await using var transaction = await connection.BeginTransactionAsync();
                var sqlCommand = connection.CreateCommand();

                foreach (var command in commands)
                {
                    sqlCommand.CommandText = command;
                    await sqlCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error Updating channels for guild {settings.GuildName}!");
                return false;
            }
        }

        public static async Task<GuildSettings> GetGuildSettings(ulong guildId)
        {
            if (Cache.ContainsKey(guildId))
                return Cache[guildId];

            GuildSettings settings = null;

            try
            {
                await using var connection = new SqliteConnection(ConnectionStringBuilder.ConnectionString);
                await connection.OpenAsync();

                var getGuildQuery = connection.CreateCommand();
                getGuildQuery.CommandText =
                    $"SELECT Disabled, RestrictTriviaMaster, GuildID, GuildName FROM {TableNameGuild} WHERE GuildID = {guildId}";

                await using var guildQueryReader = await getGuildQuery.ExecuteReaderAsync();
                
                try
                {
                    settings = new GuildSettings
                    {
                        Disabled = guildQueryReader.GetBoolean(0),
                        RestrictTrivMaster = guildQueryReader.GetBoolean(1),
                        GuildId = (ulong) guildQueryReader.GetInt64(2),
                        GuildName = guildQueryReader.GetString(3)
                    };

                    var getChannelsQuery = connection.CreateCommand();
                    getChannelsQuery.CommandText =
                        $"SELECT ChannelID, GuildID, ChannelName FROM {TableNameChannel} WHERE GuildID = {settings.GuildId}";

                    await using var channelQueryReader = await getChannelsQuery.ExecuteReaderAsync();
                    if (channelQueryReader.HasRows)
                    {
                        while (await channelQueryReader.ReadAsync())
                        {
                            settings.LockedChannels.Add(new Channel
                            {
                                ChannelId = (ulong) channelQueryReader.GetInt64(0),
                                GuildId = (ulong) channelQueryReader.GetInt64(1),
                                ChannelName = channelQueryReader.GetString(2),
                                GuildSet = settings
                            });
                        }
                    }

                    Cache.Add(settings.GuildId, settings);
                    await connection.CloseAsync();
                    return settings;
                }
                catch (InvalidOperationException ex) {}
                catch (Exception e)
                {
                    Logger.Error(e, $"Failed while attempting to retrieve Locked Channels for {settings?.GuildName}");
                }

                await connection.CloseAsync();
                if (await UpdateGuildSettings(new GuildSettings
                {
                    GuildId = guildId,
                    Disabled = false,
                    RestrictTrivMaster = true
                }))
                {
                    return await GetGuildSettings(guildId);
                }
                
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed while attempting to retrieve GuildSettings for Guild ID: {guildId}");
            }

            return settings;
        }

        public static async Task<bool> UpdateGuildSettings(GuildSettings settings)
        {
            try
            {

                var updateGuildStr = $"UPDATE {TableNameGuild} " +
                                     $"SET (Disabled, RestrictTriviaMaster) ({settings.Disabled}, {settings.RestrictTrivMaster})" +
                                     $"WHERE GuildID = {settings.GuildId}";
                
                await using var connection = new SqliteConnection(ConnectionStringBuilder.ConnectionString);
                await connection.OpenAsync();

                if (Cache.ContainsKey(settings.GuildId))
                    Cache[settings.GuildId] = settings;
                else
                {
                    updateGuildStr = $"INSERT INTO {TableNameGuild} (GuildID, Disabled, RestrictTriviaMaster, GuildName) " +
                                     $"VALUES ({settings.GuildId}, {settings.Disabled}, {settings.RestrictTrivMaster}, '{settings.GuildName}')";
                }

                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = updateGuildStr;
                await updateCommand.ExecuteNonQueryAsync();
                await UpdateChannels(settings, connection);
                await connection.CloseAsync();

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error updating Guild settings for guild {settings.GuildName}!");
                return false;
            }
        }
    }
}
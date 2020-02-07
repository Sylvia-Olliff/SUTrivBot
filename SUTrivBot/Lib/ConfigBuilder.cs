using DSharpPlus;
using DSharpPlus.CommandsNext;
using NLog;
using NLog.Config;
using NLog.Targets;
using SUTrivBot.Models;
using System;
using Microsoft.Extensions.Configuration;

namespace SUTrivBot.Lib
{
    public static class ConfigBuilder
    {
        private static Config _currentConfig = null;

        public static Config Build()
        {
            if (_currentConfig != null)
                return _currentConfig;

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var triviaSection = config.GetSection("TriviaStoreSettings");
            var commandsSection = config.GetSection("CommandsSettings");
            var botSection = config.GetSection("BotSettings");
            botSection["Token"] = config["BOT_TOKEN"];

            _currentConfig = new Config(GetLogger(), 
                new DiscordConfiguration
                {
                    Token = botSection["Token"],
                    TokenType = TokenType.Bot,
                    UseInternalLogHandler = bool.Parse(botSection["UseInternalLogHandler"]),
                    LogLevel = Enum.Parse<DSharpPlus.LogLevel>(botSection["LogLevel"]),
                    AutoReconnect = bool.Parse(botSection["AutoReconnect"])
                }, 
                new CommandsNextConfiguration
                {
                    StringPrefix = commandsSection["StringPrefix"],
                    CaseSensitive = bool.Parse(commandsSection["CaseSensitive"]),
                    EnableDefaultHelp = bool.Parse(commandsSection["EnableDefaultHelp"]),
                    EnableDms = bool.Parse(commandsSection["EnableDms"]),
                    EnableMentionPrefix = bool.Parse(commandsSection["EnableMentionPrefix"]),
                    IgnoreExtraArguments = bool.Parse(commandsSection["IgnoreExtraArguments"])
                }, 
                new TriviaStoreSettings
                {
                    PathToFile = triviaSection["PathToFile"]
                });
            
            return _currentConfig;
        }

        private static ILogger GetLogger()
        {
            var logConfig = new LoggingConfiguration();

            var fileTarget = new FileTarget()
            {
                Name = "fileLog",
                FileName = "${basedir}/Logs/info.log",
                CreateDirs = true,
                Layout = "${longdate} Thr: ${threadid} - [${level}] - ${message} ${exception}",
                ArchiveFileName = $"./Logs/Archives/{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.log",
                ArchiveOldFileOnStartup = true
            };

            var consoleTarget = new ConsoleTarget("consoleLog")
            {
                Layout = @"${date:format=HH\:mm\:ss} [${threadid}] [${level}] - ${message} ${exception}",
                OptimizeBufferReuse = true
            };

            logConfig.AddTarget(fileTarget);
            logConfig.AddTarget(consoleTarget);

            logConfig.AddRuleForAllLevels(fileTarget);
            logConfig.AddRuleForAllLevels(consoleTarget);
            LogManager.Configuration = logConfig;

            return LogManager.GetCurrentClassLogger();
        }
    }
}

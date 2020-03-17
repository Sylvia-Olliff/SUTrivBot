﻿
using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NLog;
using SUTrivBot.Models;
using SUTrivBot.Repo;

namespace SUTrivBot.Lib
{
    [Group("admin")]
    [Description("Administrative Commands")]
    [Hidden]
    [RequirePermissions(Permissions.Administrator)]
    public class AdminCommands
    {
        private readonly ILogger _logger;

        public AdminCommands()
        {
            _logger = ConfigBuilder.Build().Logger;
        }
        
        [Command("lock"), Description("Lock a channel, preventing new games from being started there")]
        public async Task LockChannel(CommandContext ctx, [Description("Channel to lock")] DiscordChannel channel)
        {
            var settings = await SettingsHandler.GetGuildSettings(ctx.Guild.Id, ctx.Guild.Name);
            var channelSettings = new Channel
            {
                ChannelId = channel.Id,
                ChannelName = channel.Name,
                GuildId = ctx.Guild.Id,
                GuildSet = settings
            };

            if (!settings.LockedChannels.Contains(channelSettings))
            {
                settings.LockedChannels.Add(channelSettings);
                await SettingsHandler.UpdateGuildSettings(settings);
            }
            await ctx.RespondAsync($"Channel: {channel.Name} locked!");
        }

        [Command("unlock"), Description("Unlock a channel, allowing new games to be started there")]
        public async Task UnLockChannel(CommandContext ctx, [Description("Channel to unlock")] DiscordChannel channel)
        {
            var settings = await SettingsHandler.GetGuildSettings(ctx.Guild.Id, ctx.Guild.Name);
            var channelSettings = new Channel
            {
                ChannelId = channel.Id,
                ChannelName = channel.Name,
                GuildId = ctx.Guild.Id,
                GuildSet = settings
            };

            if (settings.LockedChannels.Contains(channelSettings))
            {
                settings.LockedChannels.Remove(channelSettings);
                await SettingsHandler.UpdateGuildSettings(settings);
            }
            
            await ctx.RespondAsync($"Channel: {channel.Name} unlocked!");
        }

        [Command("disable"), Description("Disable PeriBot on your server")]
        public async Task DisableBot(CommandContext ctx)
        {
            var settings = await SettingsHandler.GetGuildSettings(ctx.Guild.Id, ctx.Guild.Name);
            settings.Disabled = true;
            await SettingsHandler.UpdateGuildSettings(settings);
            await ctx.RespondAsync("PeriBot Disabled!");
        }
        
        [Command("enable"), Description("Enable PeriBot on your server")]
        public async Task EnableBot(CommandContext ctx)
        {
            var settings = await SettingsHandler.GetGuildSettings(ctx.Guild.Id, ctx.Guild.Name);
            settings.Disabled = false;
            await SettingsHandler.UpdateGuildSettings(settings);
            await ctx.RespondAsync("PeriBot Enabled!");
        }

        [Command("restrict"), Description("Toggle whether or not Trivia Masters must have the trivia master role")]
        public async Task RestrictTriviaMaster(CommandContext ctx, [Description("true or false")] bool toggle)
        {
            var settings = await SettingsHandler.GetGuildSettings(ctx.Guild.Id, ctx.Guild.Name);
            settings.RestrictTrivMaster = toggle;
            await SettingsHandler.UpdateGuildSettings(settings);
            await ctx.RespondAsync($"Trivia Master restriction set to {toggle.ToString()}");
        }

        [Command("settings"), Description("Display your current settings")]
        public async Task DisplaySettings(CommandContext ctx)
        {
            try
            {
                var settings = await SettingsHandler.GetGuildSettings(ctx.Guild.Id, ctx.Guild.Name);
                var strBuilder = new StringBuilder("### SETTINGS ###\n");
                strBuilder.AppendLine($"PeriBot Enabled: {(settings.Disabled ? "No" : "Yes")}");
                strBuilder.AppendLine($"Restrict Trivia Master: {(settings.RestrictTrivMaster ? "Yes" : "No")}");
                foreach (var item in settings.LockedChannels)
                {
                    strBuilder.AppendLine($"Locked Channel: {item.ChannelName}");
                }
                await ctx.RespondAsync(strBuilder.ToString());
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error Displaying settings for Guild: {ctx.Guild.Name} in Channel: {ctx.Channel.Name}");
                await ctx.RespondAsync(
                    "OOPS! I encountered an error while attempting to retrieve your Settings, please contact my creator...");
            }
        }
    }
}
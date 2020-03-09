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
    public class AdminCommands : IDisposable
    {
        private readonly SettingsContext _context;
        private readonly ILogger _logger;

        public AdminCommands()
        {
            _context = new SettingsContext();
            _logger = ConfigBuilder.Build().Logger;
        }
        
        [Command("lock"), Description("Lock a channel, preventing new games from being started there")]
        public async Task LockChannel(CommandContext ctx, [Description("Channel to lock")] DiscordChannel channel)
        {
            var settings = await GetGuildSettings(ctx.Guild);
            settings.LockedChannels.Add(new Channel
            {
                ChannelName = channel.Name,
                GuildId = ctx.Guild.Name,
                GuildSet = settings
            });
            await _context.SaveChangesAsync();
            await ctx.RespondAsync($"Channel: {channel.Name} locked!");
        }

        [Command("disable"), Description("Disable PeriBot on your server")]
        public async Task DisableBot(CommandContext ctx)
        {
            var settings = await GetGuildSettings(ctx.Guild);
            settings.Disabled = true;
            await _context.SaveChangesAsync();
            await ctx.RespondAsync("PeriBot Disabled!");
        }
        
        [Command("enable"), Description("Enable PeriBot on your server")]
        public async Task EnableBot(CommandContext ctx)
        {
            var settings = await GetGuildSettings(ctx.Guild);
            settings.Disabled = false;
            await _context.SaveChangesAsync();
            await ctx.RespondAsync("PeriBot Enabled!");
        }

        [Command("restrict"), Description("Toggle whether or not Trivia Masters must have the trivia master role")]
        public async Task RestrictTriviaMaster(CommandContext ctx, [Description("true or false")] bool toggle)
        {
            var settings = await GetGuildSettings(ctx.Guild);
            settings.RestrictTrivMaster = toggle;
            await _context.SaveChangesAsync();
            await ctx.RespondAsync($"Trivia Master restriction set to {toggle.ToString()}");
        }

        [Command("settings"), Description("Display your current settings")]
        public async Task DisplaySettings(CommandContext ctx)
        {
            var settings = await GetGuildSettings(ctx.Guild);
            var strBuilder = new StringBuilder("### SETTINGS ###\n");
            strBuilder.AppendLine($"PeriBot Enabled: {(settings.Disabled ? "No" : "Yes")}");
            strBuilder.AppendLine($"Restrict Trivia Master: {(settings.RestrictTrivMaster ? "Yes" : "No")}");
            foreach (var item in settings.LockedChannels)
            {
                strBuilder.AppendLine($"Locked Channel: {item.ChannelName}");
            }
            await ctx.RespondAsync(strBuilder.ToString());
        }

        private async Task<GuildSettings> GetGuildSettings(DiscordGuild guild)
        {
            try
            {
                var settings = await _context.Guilds.FindAsync(guild.Name);
                if (settings != null) return settings;
                
                settings = new GuildSettings
                {
                    GuildId = guild.Name,
                    Disabled = false,
                    RestrictTrivMaster = true
                };
                await _context.Guilds.AddAsync(settings);

                return settings;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
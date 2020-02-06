using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using NLog;

namespace SUTrivBot.Models
{
    public class GameState : IGameState
    {
        private DiscordChannel _channel;
        private DiscordGuild _guild;
        private DiscordUser _triviaMaster;
        private ILogger _logger;

        public GameState(CommandContext ctx, ILogger logger)
        {
            _channel = ctx.Channel;
            _guild = ctx.Guild;
            _triviaMaster = ctx.User;
            _logger = logger;
        }

        public string GetGameName()
        {
            return $"Game in Channel {_channel.Name} in Guild {_guild.Name}";
        }
        
        public void Play(CommandContext ctx)
        {
            
        }
        
        public void Stop(CommandContext ctx)
        {
            
        }
        
        public void Next(CommandContext ctx)
        {
            
        }
    }
}
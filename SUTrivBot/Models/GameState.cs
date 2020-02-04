using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace SUTrivBot.Models
{
    public class GameState : IGameState
    {
        private CommandContext initContext; // The command context used to initialize this game
        private DiscordChannel channel;
        private DiscordGuild guild;
        private DiscordUser triviaMaster; 

        public GameState(CommandContext ctx)
        {
            initContext = ctx;
            channel = ctx.Channel;
            guild = ctx.Guild;
            triviaMaster = ctx.User;
        }

        public string getGameName()
        {
            return $"Game in Channel {channel.Name} in Guild {guild.Name}";
        }
        
        public void play(CommandContext ctx)
        {
            
        }
        
        public void stop(CommandContext ctx)
        {
            
        }
        
        public void next(CommandContext ctx)
        {
            
        }
    }
}
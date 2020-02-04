using DSharpPlus.CommandsNext;

namespace SUTrivBot.Models
{
    public interface IGameState
    {
     
        public string getGameName();
        public void play(CommandContext ctx);
        public void next(CommandContext ctx);
        public void stop(CommandContext ctx);
    }
}
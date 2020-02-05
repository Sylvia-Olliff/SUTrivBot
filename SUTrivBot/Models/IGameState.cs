using DSharpPlus.CommandsNext;

namespace SUTrivBot.Models
{
    public interface IGameState
    {
     
        public string GetGameName();
        public void Play(CommandContext ctx);
        public void Next(CommandContext ctx);
        public void Stop(CommandContext ctx);
    }
}
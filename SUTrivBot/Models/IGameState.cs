using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace SUTrivBot.Models
{
    public interface IGameState
    {
     
        public string GetGameName();
        public Task AskQuestion(CommandContext ctx);
        public Task GetResults(CommandContext ctx);
    }
}
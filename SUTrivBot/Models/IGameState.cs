using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace SUTrivBot.Models
{
    public interface IGameState
    {
     
        public string GetGameName();
        public DiscordUser GetTriviaMaster();
        public Task AskQuestion(CommandContext ctx);
        public Task GetResults(CommandContext ctx);
    }
}
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NLog;
using SUTrivBot.Models;

namespace SUTrivBot.Lib
{
    public class TriviaMasterCommands
    {
        private readonly ILogger _logger;

        public TriviaMasterCommands()
        {
            _logger = ConfigBuilder.Build().Logger;
        }

        [Command("tmEcho")]
        public async Task Echo(CommandContext ctx)
        {
            if (VerifyTriviaMaster(ctx.User))
                await ctx.RespondAsync($"Hello Trivia Master! you said: {ctx.RawArgumentString}");
        }

        [Command("getGameStatus")]
        public async Task GetGameStatus(CommandContext ctx)
        {
            if (VerifyTriviaMaster(ctx.User))
                await GameMaster.GetGameStatus(ctx, new GameId(ctx.Guild, ctx.Channel, ctx.User));
        }

        private bool VerifyTriviaMaster(DiscordUser user)
        {
            return GameMaster.GetGameByTriviaMaster(user) != null;
        }
    }
}
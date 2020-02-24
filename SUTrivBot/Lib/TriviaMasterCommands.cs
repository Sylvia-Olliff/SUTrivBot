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
        
        /**
         * Echo command for testing identifying the TriviaMaster
         */
        [Command("tmEcho")]
        public async Task Echo(CommandContext ctx)
        {
            if (VerifyTriviaMaster(ctx.User))
                await ctx.RespondAsync($"Hello Trivia Master! you said: {ctx.RawArgumentString}");
        }
        
        /**
         * Get the status of a currently running game. 
         */
        [Command("getGameStatus")]
        public async Task GetGameStatus(CommandContext ctx)
        {
            if (VerifyTriviaMaster(ctx.User))
                await GameMaster.GetGameStatus(ctx, new GameId(ctx.Guild, ctx.Channel, ctx.User));
        }

        [Command("getPoints")]
        public async Task GetGamePoints(CommandContext ctx)
        {
            if (VerifyTriviaMaster(ctx.User))
                await GameMaster.GetGameByTriviaMaster(ctx.User).GetPoints(ctx);
        }
        
        /**
         * Start a new round of trivia for a currently running game
         * Needs to check if a round is already running for this game.
         */
        [Command("start")]
        public async Task Start(CommandContext ctx)
        {
            if (!VerifyTriviaMaster(ctx.User))
                return;
            
            var game = GameMaster.GetGameByTriviaMaster(ctx.User);
            if (!(game is null))
            {
                await game.AskQuestion(ctx);
            }
            else
            {
                await ctx.RespondAsync("No Game currently running in this channel...");
            }
        }
        
        /**
         * Ends the current play session and shows the final score
         */
        [Command("stop")]
        public async Task Stop(CommandContext ctx)
        {
            if (!VerifyTriviaMaster(ctx.User))
                return;
            
            try
            {
                await GameMaster.ResolveGame(ctx, new GameId(ctx.Guild, ctx.Channel, ctx.User));
                await ctx.RespondAsync("Game over");
            }
            catch
            {
                await ctx.RespondAsync("Failed to stop game (maybe there wasn't one)");
            }
        }

        private bool VerifyTriviaMaster(DiscordUser user)
        {
            return GameMaster.GetGameByTriviaMaster(user) != null;
        }
    }
}
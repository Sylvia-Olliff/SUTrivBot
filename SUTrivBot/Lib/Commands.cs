using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using NLog;
using SUTrivBot.Models;

namespace SUTrivBot.Lib
{
    public class Commands
    {
        // TODO: Add appropriate commands. Current list: AskQuestion, GetPointsList, GetTotalQuestionCount, Echo

        private static Random _random;
        private readonly ILogger _logger;

        public Commands()
        {
            _random = new Random();
            _logger = ConfigBuilder.Build().Logger;
        }
        
        [Command("echo")]
        public async Task Echo(CommandContext ctx)
        {
            // Example Debug of a command using the logger
            _logger.Debug($"Echo Command received from {ctx.Guild.Name} in channel {ctx.Channel.Name} by user {ctx.User.Username}");
            
            await ctx.RespondAsync(ctx.RawArgumentString);
        }
        
        /**
         * [Not Role Restricted]
         * Print a list of channel related IDs
         */
        [Command("ids")]
        public async Task Ids(CommandContext ctx)
        {
            var result = "IDs " + Environment.NewLine;
            result += $"Guild ID (ctx.Guild.Id) {ctx.Guild.Id}{Environment.NewLine}";
            result += $"Channel ID (ctx.Channel.Id) {ctx.Channel.Id}{Environment.NewLine}";
            await ctx.RespondAsync(result);
        }
        
        /**
         * [Not Role Restricted]
         * Print the list of games
         */
        [Command("list")]
        public async Task List(CommandContext ctx)
        {
            var response = "";
            var gamesList = GameMaster.GetAllGames();
            response = gamesList.Count > 0 ? gamesList.Aggregate("", (acc, x) =>  $"{acc + x.GetGameName() + Environment.NewLine}") : "No games running";
            
            await ctx.RespondAsync(response);
        }

        
        /**
         * [Not Role Restricted]
         * Initiate a play session in the current channel.
         * Does nothing if a play session is already going.
         * This person is "TriviaMaster" for this game
         */
        [Command("play")]
        public async Task Play(CommandContext ctx)
        {            
                if (GameMaster.NewGame(new GameId(ctx.Guild, ctx.Channel, ctx.User)))
                    await ctx.RespondAsync("New game created");
                else
                    await ctx.RespondAsync("Failed to create new game");
        }

        /**
         * [TriviaMaster or admin]
         * Start a new round of trivia for a currently running game
         * Needs to check if a round is already running for this game.
         */
        [Command("start")]
        public async Task Start(CommandContext ctx)
        {
            var game = GameMaster.GetGameById(new GameId(ctx.Guild, ctx.Channel, null));
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
         * [TriviaMaster or admin]
         * Ends the current play session and shows the final score
         */
        [Command("stop")]
        public async Task Stop(CommandContext ctx)
        {
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

        /**
         * [Not Role Restricted]
         * Get some catpics
         */
        [Command("catpics")]
        public async Task CatPics(CommandContext ctx, [Description("How many cat pics?")] int count)
        {
            try
            {
                for (var i = 0; i < count; i++)
                {
                    var width = GetRandomNumber(500, 600);
                    var height = GetRandomNumber(500, 600);
                    await ctx.RespondAsync($"https://placekitten.com/{width}/{height}");
                }
            }
            catch
            {
                await ctx.RespondAsync("Failed to generate cat pics");
            }
        }

        /**
         * Generate a random int between 2 ints
         */
        private static int GetRandomNumber(int min, int max)  
        {  
            return _random.Next(min, max);  
        }  
    }
}

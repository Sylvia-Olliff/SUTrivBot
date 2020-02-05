using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace SUTrivBot.Models
{
    public class Commands
    {
        // TODO: Add appropriate commands. Current list: AskQuestion, GetPointsList, GetTotalQuestionCount, Echo

        private ConcurrentDictionary<string, GameState> _games;
        private static Random _random;  

        public Commands()
        {
            _games = new ConcurrentDictionary<string, GameState>();
            _random = new Random();
        }
        
        [Command("echo")]
        public async Task Echo(CommandContext ctx)
        {
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
            if (_games.Count > 0)
            {
                response = _games.Aggregate("", (acc, x) =>  $"{acc + x.Value.GetGameName() + Environment.NewLine}");
            }
            else
            {
                response = "No games running";
            }
            
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
            try
            {
                var gameState = new GameState(ctx);
                var key = GetChannelKeyFromCommandContext(ctx);
                _games.AddOrUpdate(key, gameState, (s, state) => gameState);
                await ctx.RespondAsync("New game created");
            }
            catch
            {
                await ctx.RespondAsync("Failed to create new game");
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
                var key = GetChannelKeyFromCommandContext(ctx);
                _games.TryRemove(key, out _);
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
         * Calculate the channel key based on the command context
         * This key is used to uniquely identify each game in the games dictionary
         */
        public static string GetChannelKeyFromCommandContext(CommandContext ctx)
        {
            return $"{ctx.Guild.Id}/{ctx.Channel.Id}";
        }
        
        /**
         * Generate a random int between 2 ints
         */
        public static int GetRandomNumber(int min, int max)  
        {  
            return _random.Next(min, max);  
        }  
    }
}

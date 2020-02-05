using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace SUTrivBot.Models
{
    public class Commands
    {
        // TODO: Add appropriate commands. Current list: AskQuestion, GetPointsList, GetTotalQuestionCount, Echo

        private Dictionary<string, GameState> _games;

        public Commands()
        {
            _games = new Dictionary<string, GameState>();
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
                var key = getChannelKeyFromCommandContext(ctx);
                _games.Add(key, gameState);
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
                var key = getChannelKeyFromCommandContext(ctx);
                _games.Remove(key);
                await ctx.RespondAsync("Game over");
            }
            catch
            {
                await ctx.RespondAsync("Failed to stop game (maybe there wasn't one)");
            }
        }

        /**
         * Calculate the channel key based on the command context
         * This key is used to uniquely identify each game in the games dictionary
         */
        public static string getChannelKeyFromCommandContext(CommandContext ctx)
        {
            return $"{ctx.Guild.Id}/{ctx.Channel.Id}";
        }
        
    }
}

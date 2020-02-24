using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using NLog;
using SUTrivBot.Models;

namespace SUTrivBot.Lib
{
    public class GameCommands
    {
        private readonly ILogger _logger;

        public GameCommands()
        {
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
    }
}

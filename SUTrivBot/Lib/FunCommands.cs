using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using NLog;

namespace SUTrivBot.Lib
{
    public class FunCommands
    {
        private static Random _random;
        private readonly ILogger _logger;

        public FunCommands()
        {
            _random = new Random();
            _logger = ConfigBuilder.Build().Logger;
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
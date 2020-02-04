using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SUTrivBot.Models
{
    public class Commands
    {
        // TODO: Add appropriate commands. Current list: AskQuestion, GetPointsList, GetTotalQuestionCount, Echo

        [Command("echo")]
        public async Task Echo(CommandContext ctx)
        {
            await ctx.RespondAsync($"👋 Hi, {ctx.User.Mention}!");
        }
    }
}

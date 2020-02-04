using DSharpPlus;
using DSharpPlus.CommandsNext;
using SUTrivBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SUTrivBot
{
    public class Bot : IBot
    {
        private DiscordClient discord;
        private CommandsNextModule commands;

        // TODO: Replace with ConfigBuilder and Config reference to retrieve these config values.
        public Bot(string botToken)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = botToken,
                TokenType = TokenType.Bot
            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = ";;"
            });

            commands.RegisterCommands<Commands>();
        }

        public async Task StartAsync()
        {
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}

using DSharpPlus;
using DSharpPlus.CommandsNext;
using SUTrivBot.Lib;
using SUTrivBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SUTrivBot
{
    public class Bot : IBot
    {
        private readonly DiscordClient _discord;
        private readonly CommandsNextModule _commands;

        public Bot()
        {
            var config = ConfigBuilder.Build();

            _discord = new DiscordClient(config.DiscordConfiguration);

            _commands = _discord.UseCommandsNext(config.CommandsNextConfiguration);

            _commands.RegisterCommands<Commands>();
        }

        public async Task StartAsync()
        {
            await _discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}

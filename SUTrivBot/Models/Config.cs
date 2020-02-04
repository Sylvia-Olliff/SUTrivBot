using DSharpPlus;
using DSharpPlus.CommandsNext;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Text;

namespace SUTrivBot.Models
{
    public class Config
    {
        // TODO: Place config values here. 

        public ILogger Logger { get; private set; }
        public DiscordConfiguration DiscordConfiguration { get; private set; }
        public CommandsNextConfiguration CommandsNextConfiguration { get; private set; }

        public Config(ILogger logger, DiscordConfiguration discordConfiguration, CommandsNextConfiguration commandsNextConfiguration)
        {
            Logger = logger;
            DiscordConfiguration = discordConfiguration;
            CommandsNextConfiguration = commandsNextConfiguration;
        }
    }
}

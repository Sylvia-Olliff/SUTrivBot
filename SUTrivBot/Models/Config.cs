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
        public ILogger Logger { get; }
        public DiscordConfiguration DiscordConfiguration { get; }
        public CommandsNextConfiguration CommandsNextConfiguration { get; }
        public TriviaStoreSettings TriviaStoreSettings { get; }

        public Config(ILogger logger, DiscordConfiguration discordConfiguration, CommandsNextConfiguration commandsNextConfiguration, TriviaStoreSettings triviaStoreSettings)
        {
            Logger = logger;
            DiscordConfiguration = discordConfiguration;
            CommandsNextConfiguration = commandsNextConfiguration;
            TriviaStoreSettings = triviaStoreSettings;
        }
    }
}

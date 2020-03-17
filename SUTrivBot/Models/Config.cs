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
        public ILogger Logger { get; set; }
        public DiscordConfiguration DiscordConfiguration { get; set; }
        public CommandsNextConfiguration CommandsNextConfiguration { get; set; }
        public TriviaStoreSettings TriviaStoreSettings { get; set; }
        
        public SQLSettings SqlSettings { get; set; }
    }
}

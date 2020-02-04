using DSharpPlus;
using DSharpPlus.CommandsNext;
using NLog;
using NLog.Config;
using NLog.Targets;
using SUTrivBot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SUTrivBot.Lib
{
    public static class ConfigBuilder
    {
        // TODO: Load configuration values from appsettings.json

        private static Config _currentConfig = null;

        public static Config Build()
        {
            if (_currentConfig != null)
                return _currentConfig;

            _currentConfig = new Config(GetLogger(), 
                new DiscordConfiguration
                {
                    Token = Environment.GetEnvironmentVariable("BOT_TOKEN"), // This is stored as an EnvVar for security
                    TokenType = TokenType.Bot
                },
                new CommandsNextConfiguration
                {
                    StringPrefix = ";;" // TODO: Replace this string literal with a reference to appsettings.json
                });

            return _currentConfig;
        }

        private static ILogger GetLogger()
        {
            var logConfig = new LoggingConfiguration();

            var fileTarget = new FileTarget()
            {
                Name = "fileLog",
                FileName = "${basedir}/Logs/info.log",
                CreateDirs = true,
                Layout = "${longdate} Thr: ${threadid} - [${level}] - ${message} ${exception}",
                ArchiveFileName = $"./Logs/Archives/{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.log",
                ArchiveOldFileOnStartup = true
            };

            var consoleTarget = new ConsoleTarget("consoleLog")
            {
                Layout = @"${date:format=HH\:mm\:ss} [${threadid}] [${level}] - ${message} ${exception}",
                OptimizeBufferReuse = true
            };

            logConfig.AddTarget(fileTarget);
            logConfig.AddTarget(consoleTarget);

            logConfig.AddRuleForAllLevels(fileTarget);
            logConfig.AddRuleForAllLevels(consoleTarget);
            LogManager.Configuration = logConfig;

            return LogManager.GetCurrentClassLogger();
        }
    }
}

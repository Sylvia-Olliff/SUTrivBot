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

        public Config()
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

            Logger = LogManager.GetCurrentClassLogger();
        }
    }
}

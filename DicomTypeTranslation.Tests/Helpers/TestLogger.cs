﻿
using NLog;
using NLog.Config;
using NLog.Targets;

namespace DicomTypeTranslation.Tests.Helpers
{
    public static class TestLogger
    {
        public static void Setup()
        {
            var logConfig = new LoggingConfiguration();

            var consoleTarget = new ConsoleTarget("TestConsole")
            {
                Layout = @"${level}|${message}|${exception:format=toString,Data:maxInnerExceptionLevel=5}"
            };

            logConfig.AddTarget(consoleTarget);
            logConfig.AddRuleForAllLevels(consoleTarget);

            LogManager.GlobalThreshold = LogLevel.Trace;
            LogManager.Configuration = logConfig;
            LogManager.GetCurrentClassLogger().Info("TestLogger setup, previous configuration replaced");
        }
    }
}

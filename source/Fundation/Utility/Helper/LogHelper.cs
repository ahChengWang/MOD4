using Microsoft.Extensions.Configuration;
using NLog;

namespace Utility.Helper
{
    public class LogHelper
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public LogHelper(IConfiguration config)
        {
            SetupNLog(config.GetSection("NLogSetting").GetValue<string>("LogPath"), config.GetSection("NLogSetting").GetValue<string>("Layout"));
        }

        public LogHelper()
        {

        }

        public void WriteLog(LogLevel level, string module, string msg)
        {
            switch (level.Ordinal)
            {
                case 0:
                    _logger.Trace($"{module.Replace("DomainService", "")} | {msg}");
                    break;
                case 1:
                    _logger.Debug($"{module.Replace("DomainService", "")} | {msg}");
                    break;
                case 2:
                    _logger.Info($"{module.Replace("DomainService", "")} | {msg}");
                    break;
                case 3:
                    _logger.Warn($"{module.Replace("DomainService", "")} | {msg}");
                    break;
                case 4:
                    _logger.Error($"{module.Replace("DomainService", "")} | {msg}");
                    break;
                case 5:
                default:
                    _logger.Fatal($"{module.Replace("DomainService", "")} | {msg}");
                    break;
            }
        }

        private void SetupNLog(string logPath, string layout)
        {
            var _config = new NLog.Config.LoggingConfiguration();

            var _infoLogFile = new NLog.Targets.FileTarget("f")
            {
                Name = "infofile",
                FileName = $"{logPath}info.log",
                Layout = layout
            };
            var _fatalLogFile = new NLog.Targets.FileTarget("f")
            {
                Name = "fatalfile",
                FileName = $"{logPath}fatal.log",
                Layout = layout
            };
            var _debugLogFile = new NLog.Targets.FileTarget("f")
            {
                Name = "debugfile",
                FileName = $"{logPath}debug.log",
                Layout = layout
            };

            _config.AddRule(LogLevel.Trace, LogLevel.Debug, _debugLogFile);
            _config.AddRule(LogLevel.Warn, LogLevel.Warn, _infoLogFile);
            _config.AddRule(LogLevel.Error, LogLevel.Fatal, _fatalLogFile);
            LogManager.Configuration = _config;
            _logger = LogManager.GetLogger("debug");
        }
    }
}

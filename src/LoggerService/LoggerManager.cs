using System;
using Serilog;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{

    private ILogger _logger = Log.ForContext<LoggerManager>();
    public void LogDebug(string Message)
    {
        _logger.Debug(Message);
    }

    public void LogError(string Message)
    {
        _logger.Error(Message);
    }

    public void LogInfo(string Message)
    {
        _logger.Information(Message);
    }

    public void LogWarn(string Message)
    {
        _logger.Warning(Message);
    }
}

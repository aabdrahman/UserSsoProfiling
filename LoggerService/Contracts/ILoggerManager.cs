namespace LoggerService;

public interface ILoggerManager
{
    void LogInfo(string Message);
    void LogDebug(string Message);
    void LogWarn(string Message);
    void LogError(string Message);
}


using System.Runtime.CompilerServices;

namespace LoggerService;

public interface ILoggerManager
{
    void LogInfo(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    void LogDebug(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    void LogWarn(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    void LogError(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
}


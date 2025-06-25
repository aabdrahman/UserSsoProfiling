using System.IO;
using System.Runtime.CompilerServices;
using Serilog;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{

    private ILogger _logger = Log.ForContext<LoggerManager>();
    public void LogDebug(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        var className = Path.GetFileNameWithoutExtension(callerFile);
        _logger
            .ForContext("ClassName", className)
            .ForContext("MethodName", callerName)
            .Debug(Message);
    }

    public void LogError(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        var className = Path.GetFileNameWithoutExtension(callerFile);
        _logger
            .ForContext("ClassName", className)
            .ForContext("MethodName", callerName)
            .Error(Message);
    }

    public void LogInfo(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        var className = Path.GetFileNameWithoutExtension(callerFile);
        _logger
            .ForContext("ClassName", className)
            .ForContext("MethodName", callerName)
            .Information(Message);
    }

    public void LogWarn(string Message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        var className = Path.GetFileNameWithoutExtension(callerFile);
        _logger
            .ForContext("ClassName", className)
            .ForContext("MethodName", callerName)
            .Warning(Message);
    }
}

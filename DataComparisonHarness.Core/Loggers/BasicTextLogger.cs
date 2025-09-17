namespace DataComparisonHarness.Core.Loggers;

public class BasicTextLogger<T>(T configuration) : ILogger where T : BaseConfiguration
{
    private bool logFilePrepared = false;
    public T Configuration { get; set; } = configuration;

    public string Log(string message, LogLevel level)
    {
        if (!logFilePrepared)
            PrepareLogFile();

        var levelMessage = GetLogLevelMessage(level);

        var logMessage = $"{DateTime.Now:yyyy-MM-dd--HH-mm-ss} {levelMessage}: {message}";

        File.AppendAllText(Configuration.LogFile, $"{logMessage}\r\n");

        return logMessage;
    }

    public void PrepareLogFile()
    {
        // Check/create the log directory
        if (!Directory.Exists(Configuration.LogDirectory))
            Directory.CreateDirectory(Configuration.LogDirectory);

        if (File.Exists(Configuration.LogFile))
        {
            var timestamp = File.GetCreationTime(Configuration.LogFile);
            var newFileName = $"{Configuration.LogFile}.{timestamp.ToLocalTime():yyyy-MM-dd--HH-mm-ss}";

            if (File.Exists(newFileName))
            {
                File.Delete(newFileName);
            }
            File.Move(Configuration.LogFile, newFileName);
        }

        logFilePrepared = true;
    }

    private string GetLogLevelMessage(LogLevel level)
    {
        var message = level switch
        {
            LogLevel.Info => "INFO",
            LogLevel.Warn => "WARN",
            LogLevel.Fail => "FAIL",
            LogLevel.Error => "ERR!",
            LogLevel.Fatal => "FATL",
            _ => ""
        };

        return message;
    }
}

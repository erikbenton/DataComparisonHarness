namespace DataComparisonHarness.Core.Loggers;

public interface ILogger
{
    string Log(string message, LogLevel logLevel);

    void PrepareLogFile();
}

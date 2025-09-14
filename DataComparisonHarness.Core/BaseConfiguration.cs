namespace DataComparisonHarness.Core;

public abstract class BaseConfiguration
{
    /// <summary>
    /// Name of the log file.
    /// </summary>
    public virtual string LogName { get; set; } = $"DataComparison.Log.{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.txt";

    /// <summary>
    /// Directory to save log files.
    /// </summary>
    public virtual string LogDirectory { get; set; } = @"C:\Logs\DataComparison\";

    /// <summary>
    /// Name of the log file.
    /// </summary>
    public string LogFile => $"{LogDirectory}{(!LogDirectory.EndsWith("\\") ? "\\" : "")}{LogName}";
}

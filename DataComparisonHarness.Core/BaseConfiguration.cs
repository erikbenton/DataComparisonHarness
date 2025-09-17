using System.Data;

namespace DataComparisonHarness.Core;

public abstract class BaseConfiguration
{
    /// <summary>
    /// Name of the data source with the Target data.
    /// </summary>
    public abstract string TargetDataSourceName { get; set; }

    /// <summary>
    /// Name of the data source with the Source data.
    /// </summary>
    public abstract string SourceDataSourceName { get; set; }

    /// <summary>
    /// Connection string for the database with Target data.
    /// </summary>
    public virtual string TargetConnectionString => string.Empty;

    /// <summary>
    /// Connection string for the database with the Source data.
    /// </summary>
    public virtual string SourceConnectionString => string.Empty;

    /// <summary>
    /// Columns to skip in the Target data (case-insensitive).
    /// </summary>
    public virtual List<string> TargetColumnsToSkip { get; set; } = [];

    /// <summary>
    /// Mapping for specific columns when column names differ between Target and Source (case-sensitive).
    /// </summary>
    public virtual List<(string targetColumn, string sourceColumn)> SpecificColumnMapping { get; set; } = [];

    /// <summary>
    /// Func that returns an object based on the Target data's properties/columns used to Join with the Source data.
    /// </summary>
    public abstract Func<DataRow, object> TargetJoinObject { get; set; }

    /// <summary>
    /// Func that returns an object based on the Source data's properties/columns used to Join with the Target data.
    /// </summary>
    public abstract Func<DataRow, object> SourceJoinObject { get; set; }

    public virtual double Tolerance { get; set; } = 0.00001;

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

public interface IBaseConfiguration
{
    /// <summary>
    /// Name of the data source with the Target data.
    /// </summary>
    public string TargetDataSourceName { get; set; }

    /// <summary>
    /// Name of the data source with the Source data.
    /// </summary>
    public string SourceDataSourceName { get; set; }

    /// <summary>
    /// Connection string for the database with Target data.
    /// </summary>
    public virtual string TargetConnectionString => string.Empty;

    /// <summary>
    /// Connection string for the database with the Source data.
    /// </summary>
    public virtual string SourceConnectionString => string.Empty;

    /// <summary>
    /// Columns to skip in the Target data.
    /// </summary>
    public virtual List<string> TargetColumnsToSkip => [];

    /// <summary>
    /// Name of the log file.
    /// </summary>
    public virtual string LogName => $"DataComparison.Log.{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.txt";

    /// <summary>
    /// Directory to save log files.
    /// </summary>
    public virtual string LogDirectory => @"C:\Logs\DataComparison\";

    /// <summary>
    /// Name of the log file.
    /// </summary>
    public string LogFile => $"{LogDirectory}{(!LogDirectory.EndsWith("\\") ? "\\" : "")}{LogName}";
}

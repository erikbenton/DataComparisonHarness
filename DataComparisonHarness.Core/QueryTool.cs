using DataComparisonHarness.Core.Loggers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataComparisonHarness.Core;

public class QueryTool<T>(T configuration, ILogger logger) where T : BaseConfiguration
{
    public T Configuration { get; set; } = configuration;

    public ILogger Logger { get; set; } = logger;

    public DataSet Query(string query, string configurationString, string? dataName,int commandTimeout = 1200)
    {
        var ds = new DataSet(dataName ?? "not named");
        var da = new SqlDataAdapter(query, configurationString);
        da.SelectCommand.CommandTimeout = commandTimeout;
        try
        {
            da.Fill(ds);
        }
        catch (Exception e)
        {
            Logger.Log($"{dataName ?? ""} Query Failure".TrimStart(), LogLevel.Error);
            Logger.Log("Offending Query: " + query, LogLevel.Error);
            Logger.Log("Exception Message: " + e.Message, LogLevel.Error);
            throw;
        }
        return ds;
    }

    public DataSet QuerySource(string query, int commandTimeout = 1200)
    {
        return Query(query, Configuration.SourceConnectionString, "Source", commandTimeout);
    }

    public DataSet QueryTarget(string query, int commandTimeout = 1200)
    {
        return Query(query, Configuration.TargetConnectionString, "Target", commandTimeout);
    }
}

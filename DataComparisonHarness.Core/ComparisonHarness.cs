using DataComparisonHarness.Core.Loggers;

namespace DataComparisonHarness.Core;

/// <summary>
/// Holds all of the Tools needed for testing data sources.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ComparisonHarness<T> where T : BaseConfiguration, new()
{
    protected T Configuration {  get; set; }

    public ComparisonTool<T> ComparisonTool { get; set; }

    public QueryTool<T> QueryTool { get; set; }

    public ILogger Logger { get; set; }

    public ComparisonHarness(T configuration, ILogger? logger = null)
    {
        Configuration = configuration;
        Logger = logger ?? new BasicTextLogger<T>(Configuration);
        ComparisonTool = new ComparisonTool<T>(Configuration, Logger);
        QueryTool = new QueryTool<T>(Configuration, Logger);
    }

    public ComparisonHarness(ILogger? logger = null)
    {
        Configuration = new T();
        Logger = logger ?? new BasicTextLogger<T>(Configuration);
        ComparisonTool = new ComparisonTool<T>(Configuration, Logger);
        QueryTool = new QueryTool<T>(Configuration, Logger);
    }
}

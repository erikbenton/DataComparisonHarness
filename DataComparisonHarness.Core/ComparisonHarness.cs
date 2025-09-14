using DataComparisonHarness.Core.Loggers;

namespace DataComparisonHarness.Core;

public class ComparisonHarness<T> where T : BaseConfiguration
{
    protected T Configuration {  get; set; }

    protected ComparisonEngine<T> ComparisonEngine { get; set; }

    public QueryEngine<T> QueryEngine { get; set; }

    public ILogger Logger { get; set; }

    public ComparisonHarness(T configuration, ILogger? logger = null)
    {
        Configuration = configuration;
        Logger = logger ?? new BasicTextLogger<T>(Configuration);
        ComparisonEngine = new ComparisonEngine<T>(Configuration, Logger);
        QueryEngine = new QueryEngine<T>(Configuration, Logger);
    }
}

using DataComparisonHarness.Core.Loggers;

namespace DataComparisonHarness.Core;

public class QueryEngine<T>(T configuration, ILogger logger) where T : BaseConfiguration
{
    public T Configuration { get; set; } = configuration;

    public ILogger Logger { get; set; } = logger;
}

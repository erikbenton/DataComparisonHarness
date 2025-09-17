using Newtonsoft.Json;

namespace DataComparisonHarness.Core.Utilities;

internal static class Stringify
{
    public static string ToPrettyString(this object value)
    {
        return JsonConvert.SerializeObject(value, Formatting.None);
    }
}

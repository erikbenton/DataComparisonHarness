using System.ComponentModel.DataAnnotations;

namespace DataComparisonHarness.Core;

public record ComparisonColumn(string Name, Type DataType)
{
}

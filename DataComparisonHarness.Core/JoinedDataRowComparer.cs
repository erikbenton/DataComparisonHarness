namespace DataComparisonEngine;

public class JoinedDataRowComparer : IEqualityComparer<object>
{
    public new bool Equals(object? x, object? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        var areEqual = true;
        foreach (var property in x.GetType().GetProperties())
        {
            try
            {
                var xValue = property.GetValue(x);
                var yValue = property.GetValue(y);

                if (xValue == null && yValue == null)
                {
                    areEqual = true;
                }
                else if (xValue == null || yValue == null)
                {
                    areEqual = false;
                }
                else
                {
                    areEqual &= xValue.Equals(yValue);
                }
            }
            catch
            {
                return false;
            }

            if (!areEqual) break;
        }

        return areEqual;
    }

    public int GetHashCode(object obj)
    {
        return obj.GetHashCode();
    }
}
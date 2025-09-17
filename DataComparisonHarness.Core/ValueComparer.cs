namespace DataComparisonHarness.Core;

public static class ValueComparer
{
    public static bool CompareIntegers(object a, object b)
    {
        var result = false;
        if (int.TryParse(a.ToString(), out var intA) &&
            int.TryParse(b.ToString(), out var intB))
        {
            result = intA == intB;
        }
        else
        {
            result = a == b;
        }

        return result;
    }

    public static bool CompareDoubles(object a, object b, double tolerance)
    {
        var result = false;

        if (double.TryParse(a.ToString(), out var doubleA) &&
            double.TryParse(b.ToString(), out var doubleB))
        {
            if (doubleB == 0)
            {
                result = doubleA == 0;
            }
            else
            {
                var doublePct = Math.Abs((doubleB - doubleA) / doubleB);
                result = doublePct == 0 || doublePct < tolerance;
            }
        }
        else
        {
            result = a.ToString() == b.ToString();
        }

        return result;
    }

    public static bool CompareDecimals(object a, object b, decimal tolerance)
    {
        var result = false;
        if (decimal.TryParse(a.ToString(), out var decimalA) &&
            decimal.TryParse(b.ToString(), out var decimalB))
        {

            if (decimalB == 0)
            {
                result = decimalA == 0;
            }
            else
            {
                var decimalPct = Math.Abs((decimalA - decimalB) / decimalB);
                result = decimalPct == 0 || decimalPct < tolerance;
            }
        }
        else
        {
            result = a.ToString() == b.ToString();
        }

        return result;
    }

    public static bool CompareStrings(object a, object b)
    {
        return a.ToString()!.Equals(b.ToString());
    }

    public static bool CompareDateTime(object a, object b)
    {
        return CompareStrings(a.ToString()!, b.ToString()!);
    }

    public static bool CompareBools(object a, object b)
    {
        return CompareStrings(a.ToString()!, b.ToString()!);
    }
}

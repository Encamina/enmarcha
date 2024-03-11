namespace Encamina.Enmarcha.Core;

/// <summary>
/// A utility class containing methods for mathematical operations.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Calculates the percentile value of a given sequence of doubles.
    /// </summary>
    /// <param name="values">The input sequence of double values.</param>
    /// <param name="percentile">The desired percentile value (between 0 and 100).</param>
    /// <returns>The calculated percentile value.</returns>
    public static double Percentile(IEnumerable<double> values, double percentile)
    {
        var sortedValues = values.OrderBy(x => x).ToList();
        var count = sortedValues.Count;
        var realIndex = percentile / 100.0 * (count - 1);

        var index = (int)realIndex;
        var fraction = realIndex - index;

        return ((1 - fraction) * sortedValues[index]) + (fraction * sortedValues[index + 1]);
    }

    /// <summary>
    /// Calculates the standard deviation of a given sequence of doubles.
    /// </summary>
    /// <param name="values">The input sequence of double values.</param>
    /// <returns>The calculated standard deviation.</returns>
    public static double StandardDeviation(IEnumerable<double> values)
    {
        var listValues = values.ToList();

        var mean = listValues.Average();
        var variance = listValues.Select(val => Math.Pow(val - mean, 2)).Average();

        return Math.Sqrt(variance);
    }

    /// <summary>
    /// Calculates the first and third quartiles of a given sequence of doubles.
    /// </summary>
    /// <param name="values">The input sequence of double values.</param>
    /// <returns>A tuple containing the first quartile (Q1) and third quartile (Q3).</returns>
    public static (double Q1, double Q3) Quartiles(IEnumerable<double> values)
    {
        var sortedValues = values.OrderBy(x => x).ToList();
        var count = sortedValues.Count;

        var q1 = CalculateMedian(sortedValues.Take(count / 2));
        var q3 = CalculateMedian(sortedValues.Skip((count + 1) / 2));

        return (q1, q3);
    }

    /// <summary>
    /// Calculates the Interquartile Range (IQR) of a given sequence of doubles.
    /// </summary>
    /// <param name="values">The input sequence of double values.</param>
    /// <returns>The calculated Interquartile Range (IQR).</returns>
    public static double InterquartileRange(IEnumerable<double> values)
    {
        var (q1, q3) = Quartiles(values);

        return q3 - q1;
    }

    /// <summary>
    /// Calculates the median of a collection of double values.
    /// </summary>
    /// <param name="values">The collection of double values.</param>
    /// <returns>The median of the given collection.</returns>
    public static double CalculateMedian(IEnumerable<double> values)
    {
        var listValues = values.ToList();
        var count = listValues.Count;
        var middle = count / 2;

        return count % 2 == 0 ? (listValues[middle - 1] + listValues[middle]) / 2.0 : listValues[middle];
    }
}

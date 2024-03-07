namespace Encamina.Enmarcha.Core.Tests;

public class MathUtilsTests
{
    [Theory]
    [InlineData(new double[] { 5, 5, 5, 5, 5 }, 50, 5)]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 50, 3)]
    [InlineData(new double[] { 1, 2, 3, 4, 5, 6 }, 25, 2.25)]
    [InlineData(new double[] { 1, 2, 3, 4, 5, 6 }, 75, 4.75)]
    [InlineData(new double[] { 0.5, 1.5, 2.5, 3.5, 4.5, 5.5 }, 25, 1.75)]
    public void CalculatesPercentile_Successfully(double[] values, double percentile, double expected)
    {
        var result = MathUtils.Percentile(values, percentile);
        
        Assert.Equal(expected, result, precision: 5);
    }

    [Theory]
    [InlineData(new double[] { 5, 5, 5, 5, 5 }, 0)]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 1.41421)]
    [InlineData(new double[] { 1, 2, 3, 4, 5, 6 }, 1.70783)]
    [InlineData(new double[] { 10, 20, 30, 40, 50 }, 14.14214)]
    [InlineData(new double[] { 0.1, 0.2, 0.3, 0.4, 0.5 }, 0.14142)]
    public void CalculatesStandardDeviation_Successfully(double[] values, double expected)
    {
        var result = MathUtils.StandardDeviation(values);

        Assert.Equal(expected, result, precision: 5);
    }

    [Theory]
    [InlineData(new double[] { 0, 5, 5, 5, 5 }, 2.5)]
    [InlineData(new double[] { 5, 5, 5, 5, 5 }, 0)]
    [InlineData(new double[] { 1, 2, 3, 4, 5 }, 3)]
    [InlineData(new double[] { 1, 15, 19, 64 }, 33.5)]
    [InlineData(new double[] { 10, 20, 30, 40, 50, 60 }, 30)]
    [InlineData(new double[] { 0.1, 0.2, 0.3, 0.4, 0.5 }, 0.3)]
    public void CalculatesInterquartileRange_Successfully(double[] values, double expected)
    {
        var result = MathUtils.InterquartileRange(values);

        Assert.Equal(expected, result, precision: 5);
    }
}
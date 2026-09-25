using Accrual.Domain.Commissions;

namespace Accrual.Domain.Tests;

public class CommissionCalculatorTests
{
    private static List<string> Chain(int n)
    {
        return Enumerable.Range(0, n).Select(x => x.ToString()).ToList();
    }

    [Fact]
    public void Linear_Profit1000_TenLevels()
    {
        var result = CommissionCalculator.Calculate(1000m, Chain(10), SchemaType.Linear);
        Assert.Equal(new[] { 10m, 20m, 30m, 40m, 50m, 60m, 70m, 80m, 90m, 100m },
            result.Select(c => c.Amount));
    }

    [Fact]
    public void Fibonacci_Profit1000_TenLevels()
    {
        var result = CommissionCalculator.Calculate(1000m, Chain(10), SchemaType.Fibonacci);
        Assert.Equal(new[] { 10m, 10m, 20m, 30m, 50m, 80m, 130m, 210m, 340m, 550m },
            result.Select(c => c.Amount).ToArray());
    }

    [Fact]
    public void ZeroProfit_NoCommissions()
    {
        Assert.Empty(CommissionCalculator.Calculate(0, Chain(5), SchemaType.Linear));
    }

    [Fact]
    public void NonPositiveProfit_NoCommissions()
    {
        Assert.Empty(CommissionCalculator.Calculate(-100, Chain(5), SchemaType.Linear));
    }

    [Fact]
    public void ChainLongerThanMaxDepth_IsCutAt10()
    {
        var result = CommissionCalculator.Calculate(1000m, Chain(15), SchemaType.Linear);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public void EmptyChain_NoCommissions()
    {
        Assert.Empty(CommissionCalculator.Calculate(1000m, [], SchemaType.Fibonacci));
    }

    [Fact]
    public void Levels_MapToCorrectPartners_AndStoreSchema()
    {
        var chain = Chain(3);
        var result = CommissionCalculator.Calculate(1000m, chain, SchemaType.Fibonacci);

        for (var i = 0; i < 3; i++)
        {
            Assert.Equal(chain[i], result[i].BeneficiaryExternalId);
            Assert.Equal(i + 1, result[i].Level);
            Assert.Equal(SchemaType.Fibonacci, result[i].Schema);
        }
    }
}
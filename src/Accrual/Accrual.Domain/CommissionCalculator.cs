namespace Accrual.Domain;

public static class CommissionCalculator
{
    public const int MaxDepth = 10;

    public static List<CommissionLine> Calculate(decimal profit, List<Guid> partnersUpward, SchemaType schema)
    {
        if (profit <= 0)
            return [];

        var result = new List<CommissionLine>();
        var depth = Math.Min(partnersUpward.Count, MaxDepth);

        for (var level = 1; level <= depth; level++)
        {
            var rate = schema switch
            {
                SchemaType.Linear => level,
                SchemaType.Fibonacci => Fibonacci(level),
                _ => throw new ArgumentOutOfRangeException(nameof(schema))
            };

            var amount = Math.Round(profit * rate / 100m, 4, MidpointRounding.AwayFromZero);
            if (amount > 0)
                result.Add(new CommissionLine(partnersUpward[level - 1], level, amount, schema));
        }
        
        return result;
    }

    private static int Fibonacci(int n)
    {
        var a = 1;
        var b = 1;
        for (var i = 3; i <= n; i++)
        {
            var temp = a + b;
            a = b;
            b = temp;
        }

        return b;
    }
}

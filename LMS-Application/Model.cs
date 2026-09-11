namespace LMS_Application;

public abstract class Livestock
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public double Expense { get; set; }
    public double Weight { get; set; }
    public string Colour { get; set; } = string.Empty;
    [Ignore]
    public string Type => GetType().Name;
    [Ignore]
    public abstract double DailyProfit { get; }
}

[Table("Cow")]
public class Cow : Livestock
{
    private const double MilkPrice = 9.4;
    private const double TaxPerKg = 0.02;
    public double Milk { get; set; }
    [Ignore]
    public override double DailyProfit =>
        (Milk * MilkPrice) - (Expense + (Weight * TaxPerKg));
}

[Table("Sheep")]
public class Sheep : Livestock
{
    private const double WoolPrice = 6.2;
    private const double TaxPerKg = 0.02;
    public double Wool { get; set; }

    [Ignore]
    public override double DailyProfit =>
        (Wool * WoolPrice) - (Expense + (Weight * TaxPerKg));

}

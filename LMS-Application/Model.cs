using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS_Application;

public abstract class Livestock
{
    protected const double TaxPerKg = 0.02;

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public double Expense { get; set; }
    public double Weight { get; set; }
    public string Colour { get; set; } = string.Empty;

    [Ignore]
    public string Type => GetType().Name;

    [Ignore]
    public double Tax => Weight * TaxPerKg;

    [Ignore]
    public double Cost => Expense + Tax;

    [Ignore]
    public abstract double DailyProfit { get; }
}
[Table("User")]
public class User
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Unique] public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;   // salted hash
    public bool IsAdmin { get; set; }
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
public class FarmStatistics
{
    public int SelectedCount { get; set; }
    public double Percentage { get; set; }
    public double TotalTax { get; set; }
    public double TotalProfit { get; set; }
    public double AverageWeight { get; set; }
    public double TotalMilk { get; set; }
    public double TotalWool { get; set; }

    public double TotalProduce => TotalMilk + TotalWool;

    public bool ShowDailyProfitBreakdown { get; set; }

    public double TotalDailyCowProfit { get; set; }
    public double TotalDailySheepProfit { get; set; }
    public double AverageDailyCowProfit { get; set; }
    public double AverageDailySheepProfit { get; set; }

    public double TotalDailyProfit =>
        TotalDailyCowProfit + TotalDailySheepProfit;

    
}


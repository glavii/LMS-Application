using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace LMS_Application.Pages;

public partial class LivestockStatisticsPage : ContentPage
{
    private readonly DatabaseService _dbs;

    // Same tax rate as Cow and Sheep in Model.cs.
    private const double TaxPerKg = 0.02;

    public List<string> LivestockTypes { get; set; } =
        new List<string> { "All", "Cow", "Sheep" };

    public List<string> ColourOptions { get; set; } =
        new List<string> { "All", "Red", "Black", "White" };

    public LivestockStatisticsPage(DatabaseService dbs)
    {
        InitializeComponent();
        _dbs = dbs;
        BindingContext = this;
    }

    private FarmStatistics CalculateStatistics(string type, string colour, int days)
    {
        var livestock = _dbs.ReadItems();

        var selected = livestock.Where(l =>
            (type == "All" || l.Type == type) &&
            (colour == "All" || string.Equals(l.Colour, colour,
                StringComparison.OrdinalIgnoreCase))).ToList();

        var cows = selected.OfType<Cow>().ToList();
        var sheep = selected.OfType<Sheep>().ToList();

        double dailyCowProfit = cows.Sum(c => c.DailyProfit);
        double dailySheepProfit = sheep.Sum(s => s.DailyProfit);

        return new FarmStatistics
        {
            SelectedCount = selected.Count,
            Percentage = livestock.Count == 0
                ? 0 : (double)selected.Count / livestock.Count * 100,

            TotalTax = selected.Sum(l => l.Weight * TaxPerKg) * days,

            // DailyProfit already deducts expenses and weight-based tax.
            TotalProfit = selected.Sum(l => l.DailyProfit) * days,

            AverageWeight = selected.Count == 0
                ? 0 : selected.Average(l => l.Weight),

            TotalMilk = cows.Sum(c => c.Milk) * days,
            TotalWool = sheep.Sum(s => s.Wool) * days,

            ShowDailyProfitBreakdown = type == "All" && colour == "All",
            TotalDailyCowProfit = dailyCowProfit,
            TotalDailySheepProfit = dailySheepProfit,
            AverageDailyCowProfit = cows.Count == 0
                ? 0 : dailyCowProfit / cows.Count,
            AverageDailySheepProfit = sheep.Count == 0
                ? 0 : dailySheepProfit / sheep.Count
        };
    }

    private void CalculateButton_Clicked(object? sender, EventArgs e)
    {
        ResultPanel.IsVisible = false;
        DailyProfitPanel.IsVisible = false;
        ErrorLabel.IsVisible = false;

        if (LivestockTypePicker.SelectedItem is not string type ||
            ColourPicker.SelectedItem is not string colour)
        {
            ErrorLabel.Text = "Please select a livestock type and colour.";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (!int.TryParse(ProjectionDays.Text, out int days) || days <= 0)
        {
            ErrorLabel.Text = "Please enter a positive whole number of days.";
            ErrorLabel.IsVisible = true;
            return;
        }

        FarmStatistics statistics = CalculateStatistics(type, colour, days);

        SelectedCountLabel.Text = $"Selected livestock: {statistics.SelectedCount}";
        PercentageLabel.Text = $"Percentage of farm stock: {statistics.Percentage:F2}%";
        TotalTaxLabel.Text = $"Total tax over {days} days: ${statistics.TotalTax:F2}";
        TotalProfitLabel.Text = $"Total profit / loss over {days} days: ${statistics.TotalProfit:F2}";
        AverageWeightLabel.Text = $"Average weight: {statistics.AverageWeight:F2} kg";
        TotalProduceLabel.Text = $"Total produce over {days} days (milk + wool): {statistics.TotalProduce:F2} combined units";
        TotalMilkLabel.Text = $"Total milk over {days} days: {statistics.TotalMilk:F2} litres";
        TotalWoolLabel.Text = $"Total wool over {days} days: {statistics.TotalWool:F2} kg";
        ResultPanel.IsVisible = true;

        if (statistics.ShowDailyProfitBreakdown)
        {
            TotalDailyProfitLabel.Text = $"Total daily profit: ${statistics.TotalDailyProfit:F2}";
            DailyCowProfitLabel.Text = $"Total daily cow profit: ${statistics.TotalDailyCowProfit:F2}";
            DailySheepProfitLabel.Text = $"Total daily sheep profit: ${statistics.TotalDailySheepProfit:F2}";
            AverageCowProfitLabel.Text = $"Average daily profit per cow: ${statistics.AverageDailyCowProfit:F2}";
            AverageSheepProfitLabel.Text = $"Average daily profit per sheep: ${statistics.AverageDailySheepProfit:F2}";
            DailyProfitPanel.IsVisible = true;
        }
    }
}



namespace LMS_Application.Pages;

public partial class ProfitForecastPage : ContentPage
{
    private readonly DatabaseService _dbs;

    public List<string> LivestockTypes { get; set; } =
        new List<string> { "Cow", "Sheep" };

    //public List<string> ColourOptions { get; set; } =
    //    new List<string> { "All", "black", "red", "white" };


    public ProfitForecastPage(DatabaseService dbs)
    {
        InitializeComponent();
        _dbs = dbs;
        BindingContext = this;
    }

    private double CalculateProfitForcast()
    {
        string type = LivestockTypePicker.SelectedItem as string;
        int.TryParse(Quantity.Text, out int qty);
        int.TryParse(ForecastDays.Text, out int days);

        var records = _dbs.ReadItems()
                           .Where(l => (type == "Cow" && l is Cow) || (type == "Sheep" && l is Sheep))
                           .ToList();

        if (records.Count == 0)
            return 0;

        double avgDaily = records.Average(l => l.DailyProfit);
        return avgDaily * qty * days;
    }

    private void ForecastButton_Clicked(object sender, EventArgs e)
    {
        if (LivestockTypePicker.SelectedItem is not string type)
        {
            ResultPanel.IsVisible = false;
            return; // consider showing the error some other way, e.g. a separate ErrorLabel
        }

        if (!int.TryParse(Quantity.Text, out int qty) || qty <= 0)
        {
            ResultPanel.IsVisible = false;
            return;
        }

        if (!int.TryParse(ForecastDays.Text, out int days) || days <= 0)
        {
            ResultPanel.IsVisible = false;
            return;
        }

        double totalProfit = CalculateProfitForcast();
        double avgDaily = (qty * days == 0) ? 0 : totalProfit / (qty * days);

        AvgProfitLabel.Text = $"Based on Avg Daily {type} Profit of ${avgDaily:F2}";
        TotalProfitLabel.Text = $"Buying {qty} {type.ToLower()}s would bring in estimated profit over {days} days: ${totalProfit:F0}";
        ResultPanel.IsVisible = true;
    }

    private void LivestockTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var type = LivestockTypePicker.SelectedItem as string;
       
    }
}
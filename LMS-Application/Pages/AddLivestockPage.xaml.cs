namespace LMS_Application.Pages;

public partial class AddLivestockPage : ContentPage
{
    private readonly DatabaseService _dbs;

    public List<string> LivestockTypes { get; set; } = new List<string> { "Cow", "Sheep" };
    public List<string> ColourOptions { get; set; } = new List<string> { "black", "red", "white" };

    public AddLivestockPage(DatabaseService dbs)
    {
        InitializeComponent();
        BindingContext = this;
        _dbs = dbs;
    }

    private void LivestockTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var type = LivestockTypePicker.SelectedItem as string;
        MilkEntry.IsVisible = type == "Cow";
        WoolEntry.IsVisible = type == "Sheep";
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        if (LivestockTypePicker.SelectedItem is not string type)
        {
            await DisplayAlert("", "Please select a livestock type", "OK");
            return;
        }

        if (ColourPicker.SelectedItem is not string colour)
        {
            await DisplayAlert("", "Invalid input", "OK"); // colour must be black/red/white
            return;
        }

        if (!double.TryParse(ExpenseEntry.Text, out double expense) ||
            !double.TryParse(WeightEntry.Text, out double weight))
        {
            await DisplayAlert("", "Invalid input", "OK");
            return;
        }

        Livestock record;

        if (type == "Cow")
        {
            if (!double.TryParse(MilkEntry.Text, out double milk))
            {
                await DisplayAlert("", "Invalid input", "OK");
                return;
            }

            record = new Cow
            {
                Expense = expense,
                Weight = weight,
                Colour = colour,
                Milk = milk
            };
        }
        else if (type == "Sheep")
        {
            if (!double.TryParse(WoolEntry.Text, out double wool))
            {
                await DisplayAlert("", "Invalid input", "OK");
                return;
            }

            record = new Sheep
            {
                Expense = expense,
                Weight = weight,
                Colour = colour,
                Wool = wool
            };
        }
        else
        {
            await DisplayAlert("", "Invalid livestock type", "OK");
            return;
        }
        int added = _dbs.InsertItem(record);

        if (added > 0)
        {
            WeakReferenceMessenger.Default.Send(new DBUpdateMessage(true));
            await DisplayAlert("", "Livestock added successfully", "OK");

            // clear inputs
            LivestockTypePicker.SelectedItem = null;
            ColourPicker.SelectedItem = null;
            ExpenseEntry.Text = string.Empty;
            WeightEntry.Text = string.Empty;
            MilkEntry.Text = string.Empty;
            WoolEntry.Text = string.Empty;
            MilkEntry.IsVisible = false;
            WoolEntry.IsVisible = false;
        }
        else
        {
            await DisplayAlert("", "Failed to add livestock", "OK");
        }
    }
}
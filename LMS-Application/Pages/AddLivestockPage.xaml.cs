namespace LMS_Application.Pages;

public partial class AddLivestockPage : ContentPage
{
    private readonly DatabaseService _dbs;
    private string? _type;
    private string? _colour;

    private static readonly Color Accent = Color.FromArgb("#4C8C4A");
    private static Color Neutral =>
        Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#3A3A3C")
            : Color.FromArgb("#E5E7EB");

    public AddLivestockPage(DatabaseService dbs)
    {
        InitializeComponent();
        _dbs = dbs;
    }

    private static void Mark(Border b, bool on)
    {
        b.Stroke = on ? Accent : Neutral;
        b.StrokeThickness = on ? 2 : 1;
    }

    private void SetType(string? type)
    {
        _type = type;
        Mark(CowChip, type == "Cow");
        Mark(SheepChip, type == "Sheep");
        MilkField.IsVisible = type == "Cow";
        WoolField.IsVisible = type == "Sheep";
    }

    private void SetColour(string? colour)
    {
        _colour = colour;
        Mark(BlackSwatch, colour == "black");
        Mark(RedSwatch, colour == "red");
        Mark(WhiteSwatch, colour == "white");
    }

    private void OnTypeTapped(object? sender, TappedEventArgs e) => SetType(e.Parameter as string);
    private void OnColourTapped(object? sender, TappedEventArgs e) => SetColour(e.Parameter as string);

    private async void CancelButton_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//DisplayPage");

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        if (_type is null)
        {
            await DisplayAlert("", "Please select a livestock type", "OK");
            return;
        }

        if (_colour is null)
        {
            await DisplayAlert("", "Please select a colour", "OK");
            return;
        }

        if (!double.TryParse(ExpenseEntry.Text, out double expense) ||
            !double.TryParse(WeightEntry.Text, out double weight))
        {
            await DisplayAlert("", "Invalid input", "OK");
            return;
        }

        Livestock record;

        if (_type == "Cow")
        {
            if (!double.TryParse(MilkEntry.Text, out double milk))
            {
                await DisplayAlert("", "Invalid input", "OK");
                return;
            }
            record = new Cow { Expense = expense, Weight = weight, Colour = _colour, Milk = milk };
        }
        else
        {
            if (!double.TryParse(WoolEntry.Text, out double wool))
            {
                await DisplayAlert("", "Invalid input", "OK");
                return;
            }
            record = new Sheep { Expense = expense, Weight = weight, Colour = _colour, Wool = wool };
        }

        int added = _dbs.InsertItem(record);

        if (added > 0)
        {
            WeakReferenceMessenger.Default.Send(new DBUpdateMessage(true));
            await DisplayAlert("", "Livestock added successfully", "OK");

            SetType(null);
            SetColour(null);
            ExpenseEntry.Text = string.Empty;
            WeightEntry.Text = string.Empty;
            MilkEntry.Text = string.Empty;
            WoolEntry.Text = string.Empty;
        }
        else
        {
            await DisplayAlert("", "Failed to add livestock", "OK");
        }
    }
}
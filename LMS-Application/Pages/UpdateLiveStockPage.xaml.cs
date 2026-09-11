namespace LMS_Application.Pages;

public partial class UpdateLivestockPage : ContentPage
{
    private readonly DatabaseService _dbs;
    private Livestock? _selectedItem;

    public List<string> LivestockTypes { get; set; } = new List<string> { "Cow", "Sheep" };
    public List<string> ColourOptions { get; set; } = new List<string> { "black", "red", "white" };
    public List<LivestockListItem> LivestockRecords { get; set; } = new();

    public UpdateLivestockPage(DatabaseService dbs)
    {
        InitializeComponent();
        BindingContext = this;
        _dbs = dbs;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRecordList();
    }

    private void LoadRecordList()
    {
        var items = _dbs.ReadItems();

        LivestockRecords = items
            .Select(i => new LivestockListItem
            {
                Id = i.Id,
                DisplayName = $"{(i is Cow ? "Cow" : "Sheep")} #{i.Id} – {i.Colour}"
            })
            .ToList();

        LivestockIdPicker.ItemsSource = LivestockRecords;

        // reset form since the picker's selection index resets too
        _selectedItem = null;
        SubmitButton.IsEnabled = false;
        ClearForm();
    }

    private void LivestockIdPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (LivestockIdPicker.SelectedItem is not LivestockListItem selected)
        {
            _selectedItem = null;
            SubmitButton.IsEnabled = false;
            ClearForm();
            return;
        }

        var item = _dbs.GetItemById(selected.Id);
        if (item is null)
        {
            DisplayAlert("", "Could not load that record", "OK");
            return;
        }

        _selectedItem = item;
        SubmitButton.IsEnabled = true;

        LivestockTypePicker.SelectedItem = item is Cow ? "Cow" : "Sheep";
        ColourPicker.SelectedItem = item.Colour;
        ExpenseEntry.Text = item.Expense.ToString();
        WeightEntry.Text = item.Weight.ToString();

        MilkEntry.IsVisible = item is Cow;
        WoolEntry.IsVisible = item is Sheep;

        if (item is Cow cow)
        {
            MilkEntry.Text = cow.Milk.ToString();
            WoolEntry.Text = string.Empty;
        }
        else if (item is Sheep sheep)
        {
            WoolEntry.Text = sheep.Wool.ToString();
            MilkEntry.Text = string.Empty;
        }
    }

    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        if (_selectedItem is null)
        {
            await DisplayAlert("", "Please select a livestock record first", "OK");
            return;
        }

        if (ColourPicker.SelectedItem is not string colour)
        {
            await DisplayAlert("", "Invalid input", "OK");
            return;
        }

        if (!double.TryParse(ExpenseEntry.Text, out double expense) ||
            !double.TryParse(WeightEntry.Text, out double weight))
        {
            await DisplayAlert("", "Invalid input", "OK");
            return;
        }

        Livestock record;

        if (_selectedItem is Cow)
        {
            if (!double.TryParse(MilkEntry.Text, out double milk))
            {
                await DisplayAlert("", "Invalid input", "OK");
                return;
            }

            record = new Cow
            {
                Id = _selectedItem.Id,
                Expense = expense,
                Weight = weight,
                Colour = colour,
                Milk = milk
            };
        }
        else
        {
            if (!double.TryParse(WoolEntry.Text, out double wool))
            {
                await DisplayAlert("", "Invalid input", "OK");
                return;
            }

            record = new Sheep
            {
                Id = _selectedItem.Id,
                Expense = expense,
                Weight = weight,
                Colour = colour,
                Wool = wool
            };
        }

        int updated = _dbs.UpdateItem(record);

        if (updated > 0)
        {
            WeakReferenceMessenger.Default.Send(new DBUpdateMessage(true));
            await DisplayAlert("", "Livestock updated successfully", "OK");
            LoadRecordList(); // refresh list + reset form for next edit
        }
        else
        {
            await DisplayAlert("", "Failed to update livestock", "OK");
        }
    }

    private void ClearForm()
    {
        ColourPicker.SelectedItem = null;
        ExpenseEntry.Text = string.Empty;
        WeightEntry.Text = string.Empty;
        MilkEntry.Text = string.Empty;
        WoolEntry.Text = string.Empty;
        MilkEntry.IsVisible = false;
        WoolEntry.IsVisible = false;
    }
}

public class LivestockListItem
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}
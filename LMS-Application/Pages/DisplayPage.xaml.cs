namespace LMS_Application.Pages;

public partial class DisplayPage : ContentPage
{
    private readonly DatabaseService _dbs;
    private List<Livestock> _all = [];
    private bool _ready;

    public ObservableCollection<Livestock> Livestocks { get; set; } = [];

    public DisplayPage(DatabaseService dbs)
    {
        InitializeComponent();
        BindingContext = this;
        _dbs = dbs;

        TypePicker.ItemsSource = new List<string> { "All types", "Cow", "Sheep" };
        ColourPicker.ItemsSource = new List<string> { "All colours", "Black", "Red", "White" };
        SortPicker.ItemsSource = new List<string> { "Default", "Weight", "Colour", "Cost", "Profit" };
        TypePicker.SelectedIndex = 0;
        ColourPicker.SelectedIndex = 0;
        SortPicker.SelectedIndex = 0;

        _ready = true;
        LoadFromDb();

        WeakReferenceMessenger.Default.Register<DBUpdateMessage>(this, (r, m) =>
            MainThread.BeginInvokeOnMainThread(LoadFromDb));
    }

    private void LoadFromDb()
    {
        _all = _dbs.ReadItems().ToList();
        Apply();
    }

    private void Filter_Changed(object? sender, EventArgs e)
    {
        if (_ready) Apply();
    }

    private void ClearFilters_Clicked(object? sender, EventArgs e)
    {
        _ready = false;
        TypePicker.SelectedIndex = 0;
        ColourPicker.SelectedIndex = 0;
        _ready = true;
        Apply();
    }

    private void Apply()
    {
        IEnumerable<Livestock> q = _all;

        // Filters
        if (TypePicker.SelectedItem is string type && type != "All types")
            q = q.Where(x => type == "Cow" ? x is Cow : x is Sheep);

        if (ColourPicker.SelectedItem is string colour && colour != "All colours")
            q = q.Where(x => string.Equals(x.Colour, colour, StringComparison.OrdinalIgnoreCase));

        // Sort (descending)
        q = SortPicker.SelectedItem as string switch
        {
            "Weight" => q.OrderByDescending(x => x.Weight),
            "Colour" => q.OrderByDescending(x => x.Colour),
            "Cost" => q.OrderByDescending(x => x.Cost),
            "Profit" => q.OrderByDescending(x => x.DailyProfit),
            _ => q
        };

        var list = q.ToList();

        Livestocks.Clear();
        foreach (var x in list)
            Livestocks.Add(x);

        CountLabel.Text = list.Count == _all.Count
            ? $"{_all.Count} animals"
            : $"{list.Count} of {_all.Count} animals";
    }

}
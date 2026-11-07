using Microsoft.Maui.Controls.Shapes;

namespace LMS_Application.Pages;

public partial class DataVisualisationPage : ContentPage
{
    private readonly DatabaseService _dbs;

    private static readonly Color CowColor = Color.FromArgb("#4C8C4A");
    private static readonly Color SheepColor = Color.FromArgb("#D4A017");
    private static readonly Color FallbackColor = Color.FromArgb("#9CA3AF");

    private static readonly Dictionary<string, Color> AnimalColours = new()
    {
        ["black"] = Color.FromArgb("#111827"),
        ["red"] = Color.FromArgb("#C0392B"),
        ["white"] = Colors.White
    };

    public DataVisualisationPage(DatabaseService dbs)
    {
        InitializeComponent();
        _dbs = dbs;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refresh();
    }

    private void Refresh()
    {
        var items = _dbs.ReadItems().ToList();
        bool hasData = items.Count > 0;

        EmptyState.IsVisible = !hasData;
        Dashboard.IsVisible = hasData;
        if (!hasData) return;

        var cows = items.OfType<Cow>().ToList();
        var sheep = items.OfType<Sheep>().ToList();

        // Tiles
        TotalCountLabel.Text = items.Count.ToString();
        TotalExpenseLabel.Text = items.Sum(i => i.Expense).ToString("C0");
        AvgWeightLabel.Text = $"{items.Average(i => i.Weight):0.#} kg";
        ProduceLabel.Text = $"{cows.Sum(c => c.Milk):0.#} L milk\n{sheep.Sum(s => s.Wool):0.#} kg wool";

        // Charts
        BuildBars(TypeCountChart, new[]
        {
            ("Cow", (double)cows.Count, CowColor, cows.Count.ToString()),
            ("Sheep", (double)sheep.Count, SheepColor, sheep.Count.ToString())
        });

        BuildBars(ColourChart, items
            .GroupBy(i => (i.Colour ?? "unknown").ToLower())
            .OrderByDescending(g => g.Count())
            .Select(g => (
                Cap(g.Key),
                (double)g.Count(),
                AnimalColours.TryGetValue(g.Key, out var c) ? c : FallbackColor,
                g.Count().ToString())));

        double cowExpense = cows.Sum(c => c.Expense);
        double sheepExpense = sheep.Sum(s => s.Expense);
        BuildBars(ExpenseChart, new[]
        {
            ("Cow", cowExpense, CowColor, cowExpense.ToString("C0")),
            ("Sheep", sheepExpense, SheepColor, sheepExpense.ToString("C0"))
        });

        double cowWeight = cows.Count > 0 ? cows.Average(c => c.Weight) : 0;
        double sheepWeight = sheep.Count > 0 ? sheep.Average(s => s.Weight) : 0;
        BuildBars(WeightChart, new[]
        {
            ("Cow", cowWeight, CowColor, $"{cowWeight:0.#} kg"),
            ("Sheep", sheepWeight, SheepColor, $"{sheepWeight:0.#} kg")
        });
    }

    private static string Cap(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];

    private static void BuildBars(VerticalStackLayout host,
        IEnumerable<(string Label, double Value, Color Fill, string Text)> rows)
    {
        host.Children.Clear();

        var list = rows.ToList();
        double max = list.Count == 0 ? 1 : list.Max(r => r.Value);
        if (max <= 0) max = 1;

        foreach (var r in list)
        {
            var name = new Label { Text = r.Label, FontSize = 13, VerticalOptions = LayoutOptions.Center };
            name.SetAppThemeColor(Label.TextColorProperty,
                Color.FromArgb("#1F2937"), Color.FromArgb("#F3F4F6"));

            var bar = new Border
            {
                BackgroundColor = r.Fill,
                Stroke = r.Fill == Colors.White ? Color.FromArgb("#D1D5DB") : r.Fill,
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 6 },
                HeightRequest = 22
            };

            var barGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(r.Value, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(max - r.Value, GridUnitType.Star))
                }
            };
            barGrid.Add(bar, 0, 0);

            var track = new Border
            {
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = 6 },
                HeightRequest = 22,
                VerticalOptions = LayoutOptions.Center,
                Content = barGrid
            };
            track.SetAppThemeColor(Border.BackgroundColorProperty,
                Color.FromArgb("#F0F1F3"), Color.FromArgb("#232325"));

            var value = new Label
            {
                Text = r.Text,
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalOptions = LayoutOptions.Center
            };
            value.SetAppThemeColor(Label.TextColorProperty,
                Color.FromArgb("#1F2937"), Color.FromArgb("#F3F4F6"));

            var row = new Grid
            {
                ColumnSpacing = 12,
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(80)),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(new GridLength(90))
                }
            };
            row.Add(name, 0, 0);
            row.Add(track, 1, 0);
            row.Add(value, 2, 0);

            host.Children.Add(row);
        }
    }
}
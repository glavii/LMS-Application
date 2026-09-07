using System.ComponentModel;

namespace LMS_Application.Pages;

public partial class RemoveLivestockPage : ContentPage, INotifyPropertyChanged
{
    private readonly DatabaseService _dbs;

    private ObservableCollection<Livestock> _foundRecord = new();
    public ObservableCollection<Livestock> FoundRecord
    {
        get => _foundRecord;
        set { _foundRecord = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasFoundRecord)); }
    }

    public bool HasFoundRecord => FoundRecord.Count > 0;

    public RemoveLivestockPage(DatabaseService dbs)
    {
        InitializeComponent();
        BindingContext = this;
        _dbs = dbs;
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        // 4. Format validation
        if (!int.TryParse(IdEntry.Text, out int id) || id <= 0)
        {
            FoundRecord = new ObservableCollection<Livestock>();
            await DisplayAlert("", "Invalid input", "OK");
            return;
        }

        // 3. Existence validation
        var record = _dbs.GetItemById(id); // implement in DatabaseService
        if (record is null)
        {
            FoundRecord = new ObservableCollection<Livestock>();
            await DisplayAlert("", "Non-existent livestock ID", "OK");
            return;
        }

        // show it in the reused row before confirming
        FoundRecord = new ObservableCollection<Livestock> { record };

        // 2. Confirmation
        string typeName = record is Cow ? "Cow" : "Sheep";
        bool confirm = await DisplayAlert(
            "Confirm Delete",
            $"Delete {typeName} {record.Id} - Expense:{record.Expense} Weight:{record.Weight} {record.Colour}?",
            "Yes", "No");

        if (!confirm)
            return;

        // 5. Deletion
        int deleted = _dbs.DeleteItem(id);

        if (deleted > 0)
        {
            // 6. Data consistency — refresh list page + clear this page's display
            WeakReferenceMessenger.Default.Send(new DBUpdateMessage(true));

            await DisplayAlert("", $"Record deleted: {typeName} {record.Id} " +
                $"{record.Expense} {record.Weight} {record.Colour}", "OK");

            IdEntry.Text = string.Empty;
            FoundRecord = new ObservableCollection<Livestock>();
        }
        else
        {
            await DisplayAlert("", "Failed to delete livestock", "OK");
        }
    }
}
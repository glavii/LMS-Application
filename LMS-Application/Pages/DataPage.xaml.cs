namespace LMS_Application.Pages;

public partial class DataPage : ContentPage
{
    public ObservableCollection<Livestock> Livestocks { get; set; } = [];
    public DataPage(DatabaseService dbs)
    {
        InitializeComponent();
        BindingContext = this;
        dbs.ReadItems().ForEach(x => Livestocks.Add(x));
        WeakReferenceMessenger.Default.Register<DBUpdateMessage>(this, (r, m) =>
        {
            //if(m.Value == true)
            Livestocks.Clear();
            dbs.ReadItems().ForEach(x => Livestocks.Add(x));
        }
        );
    }
}

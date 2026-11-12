namespace LMS_Application.Pages;

public partial class UserManagementPage : ContentPage
{
    private readonly AuthService _auth;
    private User? _selected;

    public UserManagementPage(AuthService auth)
    {
        InitializeComponent();
        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Refresh();
    }

    private async Task Refresh() => UsersView.ItemsSource = await _auth.GetUsersAsync();

    private void OnUserSelected(object? s, SelectionChangedEventArgs e)
    {
        _selected = e.CurrentSelection.FirstOrDefault() as User;
        if (_selected == null) return;
        UsernameEntry.Text = _selected.Username;
        PasswordEntry.Text = string.Empty;
        AdminSwitch.IsToggled = _selected.IsAdmin;
    }

    private async void OnAdd(object? s, EventArgs e) =>
        await Run(_auth.AddUserAsync(UsernameEntry.Text, PasswordEntry.Text, AdminSwitch.IsToggled));

    private async void OnEdit(object? s, EventArgs e)
    {
        if (_selected != null)
            await Run(_auth.UpdateUserAsync(_selected.Id, UsernameEntry.Text, PasswordEntry.Text, AdminSwitch.IsToggled));
    }

    private async void OnDelete(object? s, EventArgs e)
    {
        if (_selected != null) await Run(_auth.DeleteUserAsync(_selected.Id));
    }

    private async Task Run(Task<string?> op)
    {
        var error = await op;
        if (error != null) { await DisplayAlert("Error", error, "OK"); return; }
        UsernameEntry.Text = PasswordEntry.Text = string.Empty;
        _selected = null;
        await Refresh();
    }
    private void OnLogout(object? sender, EventArgs e) => AppShell.Logout();
}
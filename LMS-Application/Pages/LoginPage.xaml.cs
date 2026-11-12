namespace LMS_Application.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _auth;
    private readonly IServiceProvider _services;
    private bool _signUpMode;

    public LoginPage(AuthService auth, IServiceProvider services)
    {
        InitializeComponent();
        _auth = auth;
        _services = services;
    }

    private void OnToggleClicked(object? s, EventArgs e)
    {
        _signUpMode = !_signUpMode;
        TitleLabel.Text = _signUpMode ? "Sign up" : "Log in";
        ActionButton.Text = _signUpMode ? "Create account" : "Log in";
        ToggleButton.Text = _signUpMode ? "Have an account? Log in" : "Need an account? Sign up";
        ConfirmEntry.IsVisible = AdminRow.IsVisible = _signUpMode;
        ErrorLabel.IsVisible = false;
    }

    private async void OnActionClicked(object? s, EventArgs e)
    {
        var username = UsernameEntry.Text?.Trim() ?? "";
        var password = PasswordEntry.Text ?? "";

        if (_signUpMode)
        {
            if (password != (ConfirmEntry.Text ?? ""))
            {
                ShowMessage("Passwords don't match.", error: true);
                return;
            }

            var error = await _auth.SignUpAsync(username, password, AdminSwitch.IsToggled);
            if (error != null) { ShowMessage(error, error: true); return; }

            OnToggleClicked(null, EventArgs.Empty);        // back to login mode
            PasswordEntry.Text = ConfirmEntry.Text = string.Empty;
            AdminSwitch.IsToggled = false;
            ShowMessage("Account created. Log in.", error: false);
            return;
        }

        if (await _auth.LoginAsync(username, password))
        {
            PasswordEntry.Text = string.Empty;
            Application.Current!.Windows[0].Page = _services.GetRequiredService<AppShell>();
        }
        else ShowMessage("Invalid username or password.", error: true);
    }

    private void ShowMessage(string text, bool error)
    {
        ErrorLabel.Text = text;
        ErrorLabel.TextColor = error ? Colors.Red : Colors.Green;
        ErrorLabel.IsVisible = true;
    }
}
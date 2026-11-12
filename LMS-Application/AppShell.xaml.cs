using LMS_Application.Pages;

namespace LMS_Application
{
    public partial class AppShell : Shell
    {
        public AppShell(AuthService auth)
        {
            InitializeComponent();
            UsersTab.IsVisible = auth.IsAdmin;   // only admins see the Users tab
        }

        public static void Logout()
        {
            var services = Application.Current!.Handler.MauiContext!.Services;
            services.GetRequiredService<AuthService>().Logout();
            Application.Current.Windows[0].Page = services.GetRequiredService<LoginPage>();
        }
    }
}

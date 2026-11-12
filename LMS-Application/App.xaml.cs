using LMS_Application.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace LMS_Application
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            _services.GetRequiredService<AuthService>().InitAsync().GetAwaiter().GetResult();

            return new Window(_services.GetRequiredService<LoginPage>())
            {
                Title = "Livestock Management System",
                MinimumWidth = 900,
                MinimumHeight = 600
            };
        }
    }
}
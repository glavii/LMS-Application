using LMS_Application.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace LMS_Application
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
           
        }


        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell())
            {
                Title = "Livestock Management System",
                MinimumWidth = 900,
                MinimumHeight = 600
            };
        }
    }
}
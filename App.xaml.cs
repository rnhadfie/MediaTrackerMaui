using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            const int newHeight = 500;
            const int newWidth = 700;

            var newWindow = new Window(new AppShell())
            {
                Height = newHeight,
                Width = newWidth
            };

            return newWindow;
        }

    }
}
using MauiApp1.Components.Books;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddBook), typeof(AddBook));
        }


    }
}

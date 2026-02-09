using MauiApp1.Components.Books;
using MauiApp1.Front.Components.Series;
using MauiApp1.Front.Components.Video;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(AddBook), typeof(AddBook));
            Routing.RegisterRoute(nameof(AddSeries), typeof(AddSeries));
            Routing.RegisterRoute(nameof(AddVideo), typeof(AddVideo));
        }


    }
}

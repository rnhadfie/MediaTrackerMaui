using MauiApp1.Components.Books;
using MauiApp1.Front.Components.Books;
using MauiApp1.Front.Components.Series;
using MauiApp1.Front.Components.Video;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();


            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(BookForm), typeof(BookForm));
            Routing.RegisterRoute(nameof(SeriesForm), typeof(SeriesForm));
            Routing.RegisterRoute(nameof(VideoForm), typeof(VideoForm));
            Routing.RegisterRoute(nameof(VideoView), typeof(VideoView));
            Routing.RegisterRoute(nameof(SeriesView), typeof(SeriesView));
            Routing.RegisterRoute(nameof(BookView), typeof(BookView));


            


        }
    }
}

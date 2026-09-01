using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using MauiApp1.Front.Components.Books;
using MauiApp1.Front.Components.Books.ViewModels;
using MauiApp1.Front.Components.Videos.ViewModels;
using MauiApp1.Front.Components.Videos;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using UraniumUI;
using UraniumUI.Dialogs;


namespace MauiApp1
{
    public static class MediaTrackerMaui
    {
        public static MauiApp CreateMauiApp()
        {
            // Ensure the native provider is registered before DbContext or any SQLite use
            //SQLitePCL.Batteries_V2.Init(); 

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit(options =>
                    options.SetShouldEnableSnackbarOnWindows(true))
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseMicrocharts()
                .UseMauiCompatibility()
                .UseUraniumUIWebComponents();

           

            // Do not register MainPage as a singleton because it consumes a scoped DbContext.
            // Use transient so each activation receives a fresh scoped DbContext and to avoid
            // capturing a scoped service in a singleton which causes tracking/disposal issues.
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddCommunityToolkitDialogs();
            builder.Services.AddTransient<BookHomeViewModel>();
            builder.Services.AddTransient<BookHome>();
            // BookSeriesHome removed; use embedded BookSeriesList inside BookHome
            builder.Services.AddTransient<BookSeriesFormViewModel>();

            // Video pages and viewmodels
            builder.Services.AddTransient<VideoHomeViewModel>();
            builder.Services.AddTransient<VideoHome>();
            // VideoSeriesHome removed; use embedded VideoSeriesList inside VideoHome
            builder.Services.AddTransient<VideoForm>();
            builder.Services.AddTransient<VideoSeriesForm>();
            builder.Services.AddTransient<VideoFormViewModel>();
            builder.Services.AddTransient<VideoSeriesFormViewModel>();
            /*
            builder.Services.AddTransient<CollectionForm>();
            builder.Services.AddTransient<CollectionView>();
            builder.Services.AddTransient<CollectionDetailView>();

            builder.Services.AddTransient<BookView>();
            builder.Services.AddTransient<BookDetailView>();
            builder.Services.AddTransient<BookForm>();

          
            builder.Services.AddTransient<VideoForm>();
            builder.Services.AddTransient<VideoView>();
            builder.Services.AddTransient<VideoDetailView>();

            builder.Services.AddTransient<MusicForm>();
            builder.Services.AddTransient<MusicView>();
            builder.Services.AddTransient<MusicDetailView>();

            builder.Services.AddTransient<OtherForm>();
            builder.Services.AddTransient<OtherView>();
            builder.Services.AddTransient<OtherDetailView>();
            */


#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();
      

            return app;
        }
    }
}

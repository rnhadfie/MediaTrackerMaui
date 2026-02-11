using CommunityToolkit.Maui;
using MauiApp1.BackEnd.Database;
using MauiApp1.Components.Books;
using MauiApp1.Front.Components.Series;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Front.Components.Video;
using MauiApp1.Front.Components.Books;

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
                .UseMauiApp<App>().UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<VideoView>();
            builder.Services.AddTransient<SeriesView>();
            builder.Services.AddTransient<BookView>();

            builder.Services.AddTransient<BookForm>();
            builder.Services.AddTransient<SeriesForm>();
            builder.Services.AddTransient<VideoForm>();


            builder.Services.AddDbContext<DataContext>(
                options =>
                {
                    var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MediaTrackerSQLite.db");
                    options.UseSqlite($"Data Source={dbPath}");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                context.Database.EnsureCreated();
                if(!context.SeriesTable.Any())
                {
                   context.SeriesTable.Add(new Service.Modals.Series {
                       SeriesId = 1, 
                       Title = "Not part of a Series",
                       Author = "",
                       Artist = "",
                       Publisher = "",
                       TotalVolumes = 0,
                       CollectionStatus = Shared.Enums.CollectionStatus.NotCompleting,
                       type = Shared.Enums.MediaDataType.All });
                   context.SaveChanges();
                }
            }

            return app;
        }
    }
}

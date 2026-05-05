using CommunityToolkit.Maui;
using MauiApp1.BackEnd.Database;
using MauiApp1.Front.Components.Books;
using MauiApp1.Front.Components.Music;
using MauiApp1.Front.Components.Other;
using MauiApp1.Front.Components.Collections;
using MauiApp1.Front.Components.Video;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CollectionView = MauiApp1.Front.Components.Collections.CollectionView;
using static MauiApp1.BackEnd.Shared.Enums;

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
                });


            // Do not register MainPage as a singleton because it consumes a scoped DbContext.
            // Use transient so each activation receives a fresh scoped DbContext and to avoid
            // capturing a scoped service in a singleton which causes tracking/disposal issues.
            builder.Services.AddTransient<MainPage>();

            
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
                   context.SeriesTable.Add(new Modals.Collection {
                       SeriesId = 1, 
                       Title = "Not part of a Series",
                       Author = "",
                       Artist = "",
                       Publisher = "",
                       TotalVolumes = 0,
                       CollectionStatus = CollectionStatus.NotCompleting,
                       type = MediaDataType.All });
                   context.SaveChanges();
                }
            }

            return app;
        }
    }
}


using MauiApp1.Modals;
using Microsoft.EntityFrameworkCore;

namespace MauiApp1.BackEnd.Database
{
    public class DataContext:DbContext 
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Collection> SeriesTable { get; set; }
        public DbSet<Book> BookTable { get; set; }
        public DbSet<Video> VideoTable { get; set; }

        public DbSet<Cd> MusicTable { get; set; }
        public DbSet<BackEnd.Modals.Other> OtherTable { get; set; }
    }

}

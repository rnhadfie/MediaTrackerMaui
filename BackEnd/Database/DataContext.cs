using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Service.Modals;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace MauiApp1.BackEnd.Database
{
    public class DataContext:DbContext 
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Series> SeriesTable { get; set; }
        public DbSet<Book> BookTable { get; set; }

        public DbSet<Category> CategoryTable { get; set; }

        public DbSet<Video> VideoTable { get; set; }
    }
}

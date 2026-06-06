using MauiApp1.BackEnd.Models;
using MauiApp1.Modals;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Database
{
    public static class MediaItemDatabase
    {
        public static SQLiteAsyncConnection database;

        public static async Task Init()
        {
            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            try
            {
                var bookResult = await database.CreateTableAsync<Book>();
                var publisherResult = await database.CreateTableAsync<Publisher>();
                var bookSeriesResult = await database.CreateTableAsync<BookSeries>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
            }
        }
    }
}

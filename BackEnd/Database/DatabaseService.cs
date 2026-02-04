using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Database
{
    public class DatabaseService
    {
        SQLiteAsyncConnection _database;

        
        public async Task InitTables()
        {
            if (_database != null)
                return;

            // Initialize the connection
            _database = new SQLiteAsyncConnection(Constants.DatabasePath);

            // Create the table if it doesn't exist
             await _database.CreateTableAsync<Book>();
            await _database.CreateTableAsync<Series>();
        }

        public async Task Init()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public SQLiteAsyncConnection Database { get { return _database; } }

    }
}

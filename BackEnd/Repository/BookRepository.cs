using MauiApp1.BackEnd.Database;
using MauiApp1.Service.Modals;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Repository
{
    public class BookRepository
    {
        DatabaseService databaseService;
        SQLiteAsyncConnection database;
        public BookRepository()
        {
            databaseService = new DatabaseService();
            database = databaseService.Database;
        }
        public async Task<List<Book>> GetBooksAsync()
        {
            await databaseService.Init();
            return await database.Table<Book>().ToListAsync();
        }

        public async Task<Book> GetBookAsync(int id)
        {
            await databaseService.Init();
            return await database.Table<Book>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveBookAsync(Book item)
        {
            await databaseService.Init();
            if (item.Id != 0)
                return await database.UpdateAsync(item);
            else
                return await database.InsertAsync(item);
        }

        public async Task<int> DeleteBookAsync(Book item)
        {
            await databaseService.Init();
            return await database.DeleteAsync(item);
        }

    }
}

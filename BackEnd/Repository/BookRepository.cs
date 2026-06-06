using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Shared;
using MauiApp1.BackEnd.Interface;
using MauiApp1.Modals;
using System.Collections.ObjectModel;
using MauiApp1.BackEnd.Models;

namespace MauiApp1.BackEnd.Repository
{
    public class BookRepository: IBookRepository
    {
        public async Task<List<Book>> GetBooksAsync()
        {
            try
            {
                await MediaItemDatabase.Init();

                return await MediaItemDatabase.database.Table<Book>().ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Book>();
            }
        }

        public async Task<List<Book>> GetBooksNotReadAsync()
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.Table<Book>().Where(t => !t.Read).ToListAsync();
        }

        public async Task<Book> GetBookAsync(int id)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.Table<Book>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveBookAsync(Book item)
        {
            await MediaItemDatabase.Init();
            if (item.Id != 0)
                return await MediaItemDatabase.database.UpdateAsync(item);
            else
                return await MediaItemDatabase.database.InsertAsync(item);
        }

        public async Task<int> DeleteBookAsync(Book item)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.DeleteAsync(item);
        }

        public async Task<List<Publisher>> GetPublishersAsync()
        {
            try
            {
                await MediaItemDatabase.Init();
                return await MediaItemDatabase.database.Table<Publisher>().ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Publisher>();
            }
        }

        public async Task<int> SavePublisherAsync(Publisher item)
        {
            await MediaItemDatabase.Init();
            if (item.Id != 0)
                return await MediaItemDatabase.database.UpdateAsync(item);
            else
                return await MediaItemDatabase.database.InsertAsync(item);
        }

        public async Task<int> DeletePublisherAsync(Publisher item)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.DeleteAsync(item);
        }



        public async Task<List<BookSeries>> GetAllBookSeriesAsync()
        {
            try
            {
                await MediaItemDatabase.Init();
                return await MediaItemDatabase.database.Table<BookSeries>().ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<BookSeries>();
            }
        }
        public async Task<BookSeries> GetBookSeriesAsync(int id)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.Table<BookSeries>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveBookSeriesAsync(BookSeries item)
        {
            await MediaItemDatabase.Init();
            if (item.Id != 0)
                return await MediaItemDatabase.database.UpdateAsync(item);
            else
                return await MediaItemDatabase.database.InsertAsync(item);
        }

        public async Task<int> DeleteBookSeriesAsync(BookSeries item)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.DeleteAsync(item);
        }

        public async Task<bool> SaveBooksAsync(List<Book> books)
        {
            try
            {
                await MediaItemDatabase.Init();
                var db = MediaItemDatabase.database;
                // Use InsertOrReplace to avoid duplicates (requires primary key)
                foreach (var b in books)
                {
                    await db.InsertOrReplaceAsync(b);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SaveBookSeriesAsync(List<BookSeries> series)
        {
            try
            {
                await MediaItemDatabase.Init();
                var db = MediaItemDatabase.database;
                foreach (var s in series)
                {
                    await db.InsertOrReplaceAsync(s);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SavePublishersAsync(List<Publisher> publishers)
        {
            try
            {
                await MediaItemDatabase.Init();
                var db = MediaItemDatabase.database;
                foreach (var p in publishers)
                {
                    await db.InsertOrReplaceAsync(p);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}

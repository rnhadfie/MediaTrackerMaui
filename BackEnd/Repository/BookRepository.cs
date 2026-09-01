
using System;
using System.Diagnostics;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Interface;
using MauiApp1.BackEnd.Models;
using SQLite;

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
            return await MediaItemDatabase.database.Table<Book>().ToListAsync();
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
                item.Id = -1;
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
            try
            {
                await MediaItemDatabase.Init();
                if (item.Id != 0)
                {
                    await MediaItemDatabase.database.UpdateAsync(item);
                    return item.Id;
                }
                else
                {
                    await MediaItemDatabase.database.InsertAsync(item);
                    return item.Id;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> DeletePublisherAsync(Publisher item)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.DeleteAsync(item);
        }



        public async Task<List<BookItem>> GetAllBookSeriesAsync()
        {
            try
            {
                await MediaItemDatabase.Init();
                return await MediaItemDatabase.database.Table<BookItem>().ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<BookItem>();
            }
        }
        public async Task<BookItem> GetBookSeriesAsync(int id)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.Table<BookItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveBookSeriesAsync(BookItem item)
        {
            try
            {
                await MediaItemDatabase.Init();
                if (item.Id != 0)
                {
                    await MediaItemDatabase.database.UpdateAsync(item);
                    return item.Id;
                }
                else
                {
                    await MediaItemDatabase.database.InsertAsync(item);
                    return item.Id;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> DeleteBookSeriesAsync(BookItem item)
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

        public async Task<bool> SaveBookSeriesAsync(List<BookItem> series)
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
        public async Task<bool> SaveBookAndSeriesAsync(Book book, List<BookItem> series, string newPublisherName)
        {
            try
            {
                await MediaItemDatabase.Init();

                // Use a synchronous connection to run a transaction
                using (var conn = new SQLiteConnection(Constants.DatabasePath, Constants.Flags))
                {
                    conn.RunInTransaction(() =>
                    {
                        // If a new publisher name is provided and no publisher id set, insert publisher
                        if (!string.IsNullOrWhiteSpace(newPublisherName) && (book.Publisher == null || book.Publisher <= 0))
                        {
                            var pub = new Publisher { PublisherName = newPublisherName };
                            conn.Insert(pub);
                            book.Publisher = pub.Id;
                        }

                        // Upsert book
                        if (book.Id != 0)
                        {
                            conn.Update(book);
                        }
                        else
                        {
                            conn.Insert(book);
                        }

                        var bookId = book.Id;

                        if (series != null)
                        {
                            foreach (var s in series)
                            {
                                s.Series = bookId;
                                if (s.Id != 0)
                                    conn.Update(s);
                                else
                                    conn.Insert(s);
                            }
                        }
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log transaction failure details for diagnostics
                try
                {
                    var msg = $"SaveBookAndSeriesAsync transaction failed: {ex.Message}\n{ex.StackTrace}";
                    Console.WriteLine(msg);
                    Debug.WriteLine(msg);
                    if (ex.InnerException != null)
                    {
                        var inner = $"Inner exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}";
                        Console.WriteLine(inner);
                        Debug.WriteLine(inner);
                    }
                }
                catch
                {
                    // Swallow any logging errors to avoid secondary failures
                }

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

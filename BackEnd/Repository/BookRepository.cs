using MauiApp1.BackEnd.Database;
using MauiApp1.Service.Modals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MauiApp1.BackEnd.Repository
{
    public class BookRepository
    {
        DataContext _dbContext;

        public BookRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        public List<Book> GetAllBooks()
        {
            return new ObservableCollection<Book>(_dbContext.BookTable).ToList();
        }

        public List<Book> GetAllBooks(int take)
        {
            return new ObservableCollection<Book>(_dbContext.BookTable.Take(take)).ToList();
        }

        public Book GetBook(int id)
        {
            try
            {
                var context = _dbContext;
                var result = context.BookTable.Where(x => x.Id == id);
                return result.FirstOrDefault<Book>();
            }
            catch (Exception ex)
            {
                return new Book() { Title = "" };
            }
        }

        public bool SaveBookAsync(Book item)
        {
            try
            {
                var context = _dbContext;
                if (item.Id > 0)
                    context.BookTable.Update(item);
                else
                {
                    int maxId = 1;
                    if (context.BookTable.Any())
                    {
                        maxId = context.BookTable.Max(x => x.Id);
                    }
                    item.SeriesId = maxId + 1;

                    context.BookTable.Add(item);

                }
                int result = context.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*
        public async Task<List<Book>> GetBooksAsync()
        {
            await databaseService.Init();
           //eturn await database.Table<Book>().ToListAsync();
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
        }*/

    }
}

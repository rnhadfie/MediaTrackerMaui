using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Shared;
using MauiApp1.BackEnd.Interface;
using MauiApp1.Modals;
using System.Collections.ObjectModel;

namespace MauiApp1.BackEnd.Repository
{
    public class BookRepository: IBookRepository
    {
        DataContext _dbContext;

        public BookRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        public List<Book> GetAllBooks()
        {
            return CacheManager.GetOrAdd("books:all", () => XmlDatabase.ReadTable<Book>("BookTable"));
        }

       
        public List<Book> GetAllBooks(BookFilter filter)
        {
            var table = _dbContext.BookTable;
            IQueryable<Book> fitleredTable = null;
            if (filter.Type > 0)
            {
                fitleredTable = table.Where(x=>x.Type == filter.Type);
            }
            if (filter.Genre != null)
            {
                fitleredTable = table.Where(x => x.Genre == filter.Genre);
            }
            if (filter.Series != null)
            {
                fitleredTable = table.Where(x => x.SeriesId == filter.Series);
            }
            if (filter.Format != null)
            {
                fitleredTable = table.Where(x => x.Format == filter.Format);
            }
            if (filter.Take != null)
            {
                fitleredTable = table.Take(filter.Take.Value);
            }
            if (fitleredTable != null)
            {
                // cache filtered queries by a simple key
                var key = $"books:filter:{filter.Type}:{filter.Genre}:{filter.Series}:{filter.Format}:{filter.Take}";
                return CacheManager.GetOrAdd(key, () => new ObservableCollection<Book>(fitleredTable).ToList());
            }

            return CacheManager.GetOrAdd($"books:all:take:{filter.Take}", () => new ObservableCollection<Book>(table).ToList());
        }

        public List<Book> GetAllBooks(int take)
        {
            List<Book> books = XmlDatabase.ReadTable<Book>("BookTable");
            if (books.Count <= take) { 
                return books;
            }
            return CacheManager.GetOrAdd($"books:all:take:{take}", () => new ObservableCollection<Book>(books.GetRange(0, take)).ToList());
        }

        public Book GetBook(int id)
        {
            try
            {
                var key = $"books:id:{id}";
                return CacheManager.GetOrAdd(key, () =>
                {
                    var result = XmlDatabase.ReadTable<Book>("BookTable").Where(x => x.Id == id);
                    return result.FirstOrDefault<Book>();
                });
            }
            catch (Exception ex)
            {
                return new Book() { Title = "" };
            }
        }

        public bool SaveBook(Book item)
        {
            try
            {
                var context = _dbContext;
                if (item.Id > 0)
                    XmlDatabase.UpdateInTable<Book>("BookTable", x => x.Id == item.Id, item);
                else
                {
                    context.BookTable.Add(item);
                }
                bool success = XmlDatabase.AddToTable<Book>("BookTable", item);
                // clear relevant cache entries
                CacheManager.RemovePrefix("books:");
                return success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool DeleteBook(Book item)
        {
            try
            {
                XmlDatabase.DeleteFromTable<Book>("BookTable", x => x.Id == item.Id);
                CacheManager.RemovePrefix("books:");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}


using MauiApp1.BackEnd.Models;

namespace MauiApp1.BackEnd.Interface
{
    public interface IBookRepository
    {
        public Task<List<Book>> GetBooksAsync();

        public Task<List<Book>> GetBooksNotReadAsync();

        public Task<Book> GetBookAsync(int id);
        public Task<int> SaveBookAsync(Book item);

        public Task<int> DeleteBookAsync(Book item);
        public Task<List<Publisher>> GetPublishersAsync();
        public Task<int> SavePublisherAsync(Publisher item);
        public Task<int> DeletePublisherAsync(Publisher item);
        public Task<List<BookItem>> GetAllBookSeriesAsync();

        public Task<BookItem> GetBookSeriesAsync(int id);

        public Task<int> SaveBookSeriesAsync(BookItem item);

        public Task<int> DeleteBookSeriesAsync(BookItem item);

        public Task<bool> SaveBookSeriesAsync(List<BookItem> series);

        public Task<bool> SaveBookAndSeriesAsync(Book book, List<BookItem> series, string newPublisherName);

        public Task<bool> SavePublishersAsync(List<Publisher> publishers);

        public Task<bool> SaveBooksAsync(List<Book> books);
    }
}

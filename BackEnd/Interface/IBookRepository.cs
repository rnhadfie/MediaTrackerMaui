
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
        public Task<List<BookSeries>> GetAllBookSeriesAsync();

        public Task<BookSeries> GetBookSeriesAsync(int id);

        public Task<int> SaveBookSeriesAsync(BookSeries item);

        public Task<int> DeleteBookSeriesAsync(BookSeries item);

        public Task<bool> SaveBookSeriesAsync(List<BookSeries> series);

        public Task<bool> SavePublishersAsync(List<Publisher> publishers);

        public Task<bool> SaveBooksAsync(List<Book> books);
    }
}

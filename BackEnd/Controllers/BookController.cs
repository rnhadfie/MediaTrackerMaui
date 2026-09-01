
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Service;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Controllers.ViewModels;

namespace MauiApp1.Controllers
{
    public class BookController
    {

        private Lazy<BookService> BookService;
        private BookService _BookService;

        private Lazy<SharedService> SharedService;
        private SharedService _SharedService;

        public BookController()
        {
            BookService = new Lazy<BookService>(() => new BookService());
            _BookService = BookService.Value;

            SharedService = new Lazy<SharedService>(() => new SharedService());
            _SharedService = SharedService.Value;
        }

        public async Task<BookSetupViewModel> GetBooksAsync()
        {
            var books = await _BookService.GetBooksAsync();
            var series = await _BookService.GetAllBookSeriesAsync();
            var genres = _SharedService.GetGenres();
            var formats = _BookService.GetBookFormats();
            var types = _BookService.GetBookTypes();
            var languages = _SharedService.GetLanguages();
            var publishers = await _BookService.GetPublishersAsync();

            BookSetupViewModel bookSetupViewModel = new BookSetupViewModel
            {
                Books = new System.Collections.ObjectModel.ObservableCollection<Book>(books),
                BookSeries = new System.Collections.ObjectModel.ObservableCollection<BookItem>(series),
                Genre = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string,int>>(genres),
                Format = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string,int>>(formats),
                Type = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string,int>>(types),
                Publisher = new System.Collections.ObjectModel.ObservableCollection<Publisher>(publishers)
            };

            return bookSetupViewModel;
        }

        public async Task<BookSetupViewModel> GetBooksNotReadAsync()
        {
            var books = await _BookService.GetBooksNotReadAsync();
            var series = await _BookService.GetAllBookSeriesAsync();
            var genres = _SharedService.GetGenres();
            var formats = _BookService.GetBookFormats();
            var types = _BookService.GetBookTypes();
            var languages = _SharedService.GetLanguages();
            var publishers = await _BookService.GetPublishersAsync();

            BookSetupViewModel bookSetupViewModel = new BookSetupViewModel
            {
                Books = new System.Collections.ObjectModel.ObservableCollection<Book>(books),
                BookSeries = new System.Collections.ObjectModel.ObservableCollection<BookItem>(series),
                Genre = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string,int>>(genres),
                Format = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string,int>>(formats),
                Type = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string,int>>(types),
                Publisher = new System.Collections.ObjectModel.ObservableCollection<Publisher>(publishers)
            };
            return bookSetupViewModel;
        }

        public async Task<Book> GetBookAsync(int id)
        {
            return await _BookService.GetBookAsync(id);
        }

        public async Task<bool> SaveBookAsync(Book item, List<BookItem> items, string newSeries, string newPublisher)
        {
            // Delegate to service which will run a transactional save for book, its series and optional publisher creation
            var ok = await _BookService.SaveBookAndSeriesAsync(item, items, newPublisher);
            return ok;
        }

        public async Task<int> DeleteBookAsync(Book item)
        {
            return await _BookService.DeleteBookAsync(item);
        }

        public async Task<List<Publisher>> GetPublishersAsync()
        {
            return await _BookService.GetPublishersAsync();
        }

        public async Task<int> SavePublisherAsync(Publisher item)
        {
            return await _BookService.SavePublisherAsync(item);
        }

        public async Task<int> DeletePublisherAsync(Publisher item)
        {
            return await _BookService.DeletePublisherAsync(item);
        }

        public async Task<BookItem> GetBookSeriesAsync(int id)
        {
            return await _BookService.GetBookSeriesAsync(id);
        }

        public async Task<List<BookItem>> GetAllBookSeriesAsync()
        {
            return await _BookService.GetAllBookSeriesAsync();
        }

        public async Task<int> SaveBookSeriesAsync(BookItem item)
        {
            await MediaItemDatabase.Init();
            if (item.Id != 0)
                return await MediaItemDatabase.database.UpdateAsync(item);
            else
                return await MediaItemDatabase.database.InsertAsync(item);
        }

        public async Task<int> DeleteBookSeriesAsync(BookItem item)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.DeleteAsync(item);
        }

        public async Task<BookSetupViewModel> GetBookSetup()
        {
            var series = await _BookService.GetAllBookSeriesAsync();
            var genres = _SharedService.GetGenres();
            var formats = _BookService.GetBookFormats();
            var types = _BookService.GetBookTypes();
            var languages = _SharedService.GetLanguages();
            var publishers = await _BookService.GetPublishersAsync();

            BookSetupViewModel setup = new BookSetupViewModel
            {
                BookSeries = new System.Collections.ObjectModel.ObservableCollection<BookItem>(series),
                Genre = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(genres),
                Format = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(formats),
                Type = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(types),
                Publisher = new System.Collections.ObjectModel.ObservableCollection<Publisher>(publishers)
            };
            return setup;
        }

        public async Task<Book> GetBook(int id)
        {
            if (id <= 0) return null;
            return await _BookService.GetBookAsync(id);
        }
    }
}

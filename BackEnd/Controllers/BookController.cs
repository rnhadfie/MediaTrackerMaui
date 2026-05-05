using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Modals;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Controllers
{
    public class BookController
    {
        DataContext _dataContext;
        private Lazy<SeriesService> SeriesService;
        private SeriesService _SeriesService;

        private Lazy<BookService> BookService;
        private BookService _BookService;
        public BookController(DataContext dataContext) {
           _dataContext = dataContext;
            _SeriesService = new Lazy<SeriesService>(() =>
            {
                return new SeriesService(_dataContext);
            }).Value;

            _BookService = new Lazy<BookService>(() =>
            {
                return new BookService(_dataContext);
            }).Value;
        }

        public BookSetupViewModel GetBookSetup() {
            BookSetupViewModel viewModel
                = new BookSetupViewModel();
            viewModel.Series = _SeriesService.GetListOfSeries(MediaDataType.Book);
            viewModel.Genre = _BookService.GetGenres();
            viewModel.Format = _BookService.GetBookFormats();
            viewModel.Type = _BookService.GetBookTypes();

            return viewModel;
        }

        public bool AddBook(Book newBook)
        {
            return _BookService.AddNewBook(newBook);
        }


        public Book GetBookInfo(int id)
        {
            return _BookService.GetBookInfo(id);
        }

        public Collection GetSeriesInfo(int id)
        {
            return _SeriesService.GetSeries(id);
        }

        public List<Book> GetAllBooks(BookFilter filter)
        {
             return _BookService.GetBookList(filter);
        }
    }
}

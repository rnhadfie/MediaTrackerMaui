using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.BackEnd.Repository;
using MauiApp1.Modals;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    public class BookService
    {
        private Lazy<BookRepository> BookRepository;
        private BookRepository _BookRepository;

        public BookService(DataContext dataContext)
        {
            _BookRepository = new Lazy<BookRepository>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new BookRepository(dataContext);
            }).Value;
        }
        public bool AddNewBook(Book book)
        {
            byte[] CompressedImageData = book.Cover ?? [];
            if (CompressedImageData.Length > 0)
            {
                book.Cover = SharedService.CompressImage(CompressedImageData, 100, 60);
            }
            return _BookRepository.SaveBook(book);
        }


        public List<DisplayViewItem> GetAllBooks(int take)
        {
            List<Book> bookList = _BookRepository.GetAllBooks(take);
            List<DisplayViewItem> bookDisplayList = new List<DisplayViewItem>();
            bookList.ForEach(x =>
            {
                bookDisplayList.Add(new DisplayViewItem()
                {
                    Id = x.Id,
                    Name = x.Title,
                    Cover = x.Cover,
                    Type = MediaDataType.Book

                });
            });
            return bookDisplayList;
        }

        public List<Book> GetBookList(BookFilter filter)
        {
           List<Book> books = _BookRepository.GetAllBooks();
            books.FindAll(x =>
            {
                bool matches = true;
                if (filter.Genre.HasValue)
                {
                    matches &= x.Genre == filter.Genre.Value;
                }
                if (filter.Format.HasValue)
                {
                    matches &= x.Format == filter.Format.Value;
                }
                if (filter.Type.HasValue)
                {
                    matches &= x.Type == filter.Type.Value;
                }
                if (filter.Series.HasValue)
                {
                    matches &= x.SeriesId == filter.Series.Value;
                }
                return matches;
            });

            books.GetRange(filter.Take ?? 0, books.Count - (filter.Take ?? 0));
            return books;
        }

        public Book GetBookInfo(int id)
        {
            List<Book> books = _BookRepository.GetAllBooks();
            return _BookRepository.GetBook(id);
        }

        public List<TextValuePair<int>> GetGenres()
        {
            var bookGenres = new List<TextValuePair<int>>
            {
                new TextValuePair<int>("Fiction", 1),
                new TextValuePair<int>("Non-Fiction", 2),
                new TextValuePair<int>("Science Fiction",  3 ),
                new TextValuePair<int>("Fantasy", 4 ),
                new TextValuePair<int>("Biography",  5 ),
                new TextValuePair<int>("History",  6 ),
                new TextValuePair<int>("Mystery", 7 ),
                new TextValuePair<int>("Romance", 8 ),
                new TextValuePair<int>("Thriller", 9 ),
                new TextValuePair<int>("Self-Help", 10 )
            };
            return bookGenres;

        }

        public List<TextValuePair<int>> GetBookFormats()
        {
            var bookFormats = new List<TextValuePair<int>>
            {
                new TextValuePair<int>("Paperback", 1),
                new TextValuePair<int>("Hardcover", 2),
                new TextValuePair<int>("E-Book", 3)
            };
            return bookFormats;
        }

        public List<TextValuePair<int>> GetBookTypes()
        {
            var bookTypes = new List<TextValuePair<int>>
            {
                new TextValuePair<int>("Novel", 1),
                new TextValuePair<int>("Anthology", 2),
                new TextValuePair<int>("Graphic Novel", 3),
                new TextValuePair<int>("Manga", 4)
            };
            return bookTypes;
        }
    }
}

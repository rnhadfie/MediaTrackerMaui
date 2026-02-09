using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Repository;
using MauiApp1.Repository;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using System;
using System.Collections.Generic;
using System.Text;

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
            book.Cover = SharedService.CompressImage(CompressedImageData, 100, 60);
            return _BookRepository.SaveBookAsync(book);
        }

        public List<TextValuePair<int>> GetGenres()
        {
            var bookGenres = new List<TextValuePair<int>>
            {
                new TextValuePair<int>("Fiction", 1),
                new TextValuePair<int>("Non-Fiction", 2),
                new TextValuePair<int>("Science Fiction",  3 ),
                new TextValuePair<int>("Fantasy", 4 ),
                new TextValuePair<int>( "Biography",  5 ),
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

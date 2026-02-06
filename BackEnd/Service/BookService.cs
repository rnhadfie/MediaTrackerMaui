using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Repository;
using MauiApp1.Repository;
using MauiApp1.Service.Modals;
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
        public bool AddNewBool(Book book)
        {
            return _BookRepository.SaveBookAsync(book);
        }
    }
}

using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.Modals;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Interface
{
    public interface IBookRepository
    {
        public List<Book> GetAllBooks();

        //public List<Book> GetAllBooks(BookFilter filter);

        public List<Book> GetAllBooks(int take);

        public Book GetBook(int id);

        public bool SaveBook(Book item);
        public bool DeleteBook(Book item);
    }
}

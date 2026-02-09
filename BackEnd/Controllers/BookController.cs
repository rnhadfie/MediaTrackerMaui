using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Text;

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
            viewModel.Series = _SeriesService.GetListOfSeries();
            viewModel.Genre = _BookService.GetGenres();
            viewModel.Format = _BookService.GetBookFormats();
            viewModel.Type = _BookService.GetBookTypes();

            return viewModel;
        }

        public bool AddBook(Book newBook)
        {
            return _BookService.AddNewBook(newBook);
        }

        public Series GetSeriesInfo(int id)
        {
            return _SeriesService.GetSeries(id);
        }
    }
}

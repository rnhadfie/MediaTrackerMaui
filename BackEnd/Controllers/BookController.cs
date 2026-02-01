using MauiApp1.Controllers.ViewModels;
using MauiApp1.Service;
using MauiApp1.Shared;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Controllers
{
    public class BookController
    {
        private Lazy<GeneralService> GeneralService;
        private GeneralService _GeneralService;
        public BookController() {
            _GeneralService = new Lazy<GeneralService>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new GeneralService();
            }).Value;
        }

        public BookSetupViewModel GetBookSetup() {
            BookSetupViewModel viewModel
                = new BookSetupViewModel();
            viewModel.ListOfSeries = _GeneralService.GetListOfSeries();

            return viewModel;
        }
    }
}

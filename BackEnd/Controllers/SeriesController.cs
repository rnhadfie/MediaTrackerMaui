using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Service.Modals;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Controllers
{
    public class SeriesController
    {
        private Lazy<SeriesService> SeriesService;
        private SeriesService _SeriesService;
        public SeriesController(DataContext dataContext)
        {
            _SeriesService = new Lazy<SeriesService>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new SeriesService(dataContext);
            }).Value;
        }

        public SeiresSetupViewModel GetSeriesSetup()
        {
            SeiresSetupViewModel viewModel
                    = new SeiresSetupViewModel();
            viewModel.ListOfSeriesStatus = _SeriesService.GetCollectionStatus();

            return viewModel;
        }

        public bool SaveNewSeries(Series newSeries)
        {
            _SeriesService.AddNewSeries(newSeries);
            return false;
        }
    }
}


using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Service;
using MauiApp1.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Controllers
{
    public class SeriesController
    {
        private Lazy<SeriesService> SeriesService;
        private SeriesService _SeriesService;
        public SeriesController()
        {
            _SeriesService = new Lazy<SeriesService>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new SeriesService();
            }).Value;
        }

        public SeiresSetupViewModel GetSeriesSetup()
        {
            SeiresSetupViewModel viewModel
                    = new SeiresSetupViewModel();
            viewModel.ListOfSeries = _SeriesService.GetListOfSeries();
            viewModel.ListOfSeriesStatus = _SeriesService.GetCollectionStatus();

            return viewModel;
        }
    }
}


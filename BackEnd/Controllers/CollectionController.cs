using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.BackEnd.Service;
using MauiApp1.Modals;

namespace MauiApp1.BackEnd.Controllers
{
    public class CollectionController
    {
        private Lazy<SeriesService> SeriesService;
        private SeriesService _SeriesService;
        public CollectionController(DataContext dataContext)
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
            viewModel.ListOfMediaTypes = _SeriesService.GetSertiesType();

            return viewModel;
        }

        public List<DisplayViewItem> GetSeriesDisplayViewList()
        {
            return _SeriesService.GetDisplaySeriesList();
        }

        public List<Collection> GetSeriesList()
        {
            return _SeriesService.GetSeriesList();
        }

        public Collection GetSeriesInfo(int id)
        {
            return _SeriesService.GetSeries(id);
        }

        public bool SaveNewSeries(Collection newSeries)
        {
            _SeriesService.AddNewSeries(newSeries);
            return false;
        }


        public List<DisplayViewItem> GetSeriesItems(int seriesId)
        {
            return _SeriesService.GetSeriesItems(seriesId);
        }
    }
}


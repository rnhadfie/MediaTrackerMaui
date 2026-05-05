using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.BackEnd.Service;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Modals;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    class OtherController
    {
        DataContext _dataContext;
        private Lazy<SeriesService> SeriesService;
        private SeriesService _SeriesService;

        private Lazy<OtherService> OtherService;
        private OtherService _OtherService;
        public OtherController(DataContext dataContext)
        {
            _dataContext = dataContext;
            _SeriesService = new Lazy<SeriesService>(() =>
            {
                return new SeriesService(_dataContext);
            }).Value;

            _OtherService = new Lazy<OtherService>(() =>
            {
                return new OtherService(_dataContext);
            }).Value;
        }

        public OtherViewModel GetOtherSetup()
        {
            OtherViewModel viewModel
                = new OtherViewModel();
            viewModel.Collection = _SeriesService.GetListOfSeries(Enums.MediaDataType.Other);

            return viewModel;
        }

        public bool AddOther(Other otherItem)
        {
            return _OtherService.AddOther(otherItem);
        }


        public Other GetOtherInfo(int id)
        {
            return _OtherService.GetOtherItemInfo(id);
        }

        public Collection GetSeriesInfo(int id)
        {
            return _SeriesService.GetSeries(id);
        }
        
        public List<Other> GetAllOtehrItems(int take = -1)
        {
            return _OtherService.GetOtherItems(take);
        }
    }
}

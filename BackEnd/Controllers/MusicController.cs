using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Modals;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    public class MusicController
    {
        DataContext _dataContext;
        private Lazy<SeriesService> SeriesService;
        private SeriesService _SeriesService;

        private Lazy<MusicSerivce> MusicService;
        private MusicSerivce _MusicService;
        public MusicController(DataContext dataContext)
        {
            _dataContext = dataContext;
            _SeriesService = new Lazy<SeriesService>(() =>
            {
                return new SeriesService(_dataContext);
            }).Value;

            _MusicService = new Lazy<MusicSerivce>(() =>
            {
                return new MusicSerivce(_dataContext);
            }).Value;
        }

        public MusicSetupViewModel GetMusicSetup()
        {
            MusicSetupViewModel viewModel
                = new MusicSetupViewModel();
            viewModel.Collection = _SeriesService.GetListOfSeries(Enums.MediaDataType.Cd);
            viewModel.Genres = _MusicService.GetGenres();
            viewModel.Langauge = _MusicService.GetLanguage();

            return viewModel;
        }

        public bool AddMusic(Cd cd)
        {
            return _MusicService.AddMusic(cd);
        }


        public Cd GetMusicInfo(int id)
        {
            return _MusicService.GetMusicInfo(id);
        }

        public Collection GetSeriesInfo(int id)
        {
            return _SeriesService.GetSeries(id);
        }
        
        public List<Cd> GetMusicItems(int take)
        {
            return _MusicService.GetCdList(take);
        }
    }
}

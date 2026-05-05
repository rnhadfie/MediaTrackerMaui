using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service;
using MauiApp1.Modals;

namespace MauiApp1.BackEnd.Controllers
{
    public class VideoController
    {
        DataContext _dataContext;
        private SeriesService _SeriesService;
        private VideoService _VideoService;

        public VideoController(DataContext dataContext)
        {
            _dataContext = dataContext;
            _SeriesService = new Lazy<SeriesService>(() =>
            {
                return new SeriesService(_dataContext);
            }).Value;

            _VideoService = new Lazy<VideoService>(() =>
            {
                return new VideoService(_dataContext);
            }).Value;
        }

        public VideoSetupViewModel GetVideoSetup()
        {
            VideoSetupViewModel viewModel
                = new VideoSetupViewModel();
            viewModel.Series = _SeriesService.GetListOfSeries();
            viewModel.Category = _VideoService.GetVideoGroups();
            viewModel.VideoFormat = _VideoService.GetVideoFormat();
            viewModel.VideoType = _VideoService.GetVideoType();
            viewModel.Genre = _VideoService.GetGenres();


            return viewModel;
        }

        public bool AddVideo(Video newVideo)
        {
            return _VideoService.AddVideo(newVideo);
        }

        public Video GetVideoInfo(int id)
        {
            return _VideoService.GetVideoInfo(id);
        }

        public List<Video> GetAllVideos(VideoFitler filter)
        {
            return _VideoService.GetAllVideos(filter);
        }
    }
}

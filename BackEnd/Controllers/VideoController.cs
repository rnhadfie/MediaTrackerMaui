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


            return viewModel;
        }

        public bool AddVideo(Video newVideo)
        {
            return _VideoService.AddVideo(newVideo);
        }
    }
}

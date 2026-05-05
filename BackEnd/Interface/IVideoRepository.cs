using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.Modals;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Interface
{
    public interface IVideoRepository
    {
        public List<Video> GetAllVideo();

        public List<Video> GetAllVideo(int take);

        public List<Video> GetAllVideo(VideoFitler filter);

        public Video GetVideo(int id);

        public bool SaveVideo(Video item);

        public bool DeleteVideo(Video item);
    }
}

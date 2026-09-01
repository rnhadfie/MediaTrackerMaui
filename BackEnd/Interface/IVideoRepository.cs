using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Interface
{
    public interface IVideoRepository
    {
        public Task<List<Video>> GetVideosAsync();

        public Task<List<Video>> GetVideosNotReadAsync();

        public Task<Video> GetVideoAsync(int id);
        public Task<int> SaveVideoAsync(Video item);

        public Task<int> DeleteVideoAsync(Video item);
        public Task<List<VideoSeries>> GetAllVideoSeriesAsync();

        public Task<VideoSeries> GetVideoSeriesAsync(int id);

        public Task<int> SaveVideoSeriesAsync(VideoSeries item);

        public Task<int> DeleteVideoSeriesAsync(VideoSeries item);

        public Task<bool> SaveVideoSeriesAsync(List<VideoSeries> series);

        public Task<bool> SaveVideosAsync(List<Video> videos);
    }
}

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
        public Task<List<VideoItem>> GetAllVideoSeriesAsync();

        public Task<VideoItem> GetVideoSeriesAsync(int id);

        public Task<int> SaveVideoSeriesAsync(VideoItem item);

        public Task<int> DeleteVideoSeriesAsync(VideoItem item);

        public Task<bool> SaveVideoSeriesAsync(List<VideoItem> series);

        public Task<bool> SaveVideosAsync(List<Video> videos);
    }
}

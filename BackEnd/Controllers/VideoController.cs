using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using MauiApp1.BackEnd.Service;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    public class VideoController
    {
        private Lazy<VideoService> VideoService;
        private VideoService _VideoService;

        private Lazy<SharedService> SharedService;
        private SharedService _SharedService;

        public VideoController()
        {
            VideoService = new Lazy<VideoService>(() => new VideoService());
            _VideoService = VideoService.Value;

            SharedService = new Lazy<SharedService>(() => new SharedService());
            _SharedService = SharedService.Value;
        }

        public async Task<VideoSetupViewModel> GetVideosAsync()
        {
            var videos = await _VideoService.GetVideosAsync();
            var series = await _VideoService.GetAllVideoSeriesAsync();
            var genres = _SharedService.GetGenres();
            var formats = _VideoService.GetVideoFormats();
            var types = _VideoService.GetVideoTypes();
            var languages = _SharedService.GetLanguages();

            VideoSetupViewModel videoSetupViewModel = new VideoSetupViewModel
            {
                Video = new System.Collections.ObjectModel.ObservableCollection<Video>(videos),
                VideoSeries = new System.Collections.ObjectModel.ObservableCollection<VideoSeries>(series),
                Genre = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(genres),
                Format = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(formats),
                Type = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(types),
                Langauge = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(languages)
            };

            return videoSetupViewModel;
        }

        public async Task<VideoSetupViewModel> GetBooksNotReadAsync()
        {
            var videos = await _VideoService.GetVideosNotReadAsync();
            var series = await _VideoService.GetAllVideoSeriesAsync();
            var genres = _SharedService.GetGenres();
            var formats = _VideoService.GetVideoFormats();
            var types = _VideoService.GetVideoTypes();
            var languages = _SharedService.GetLanguages();

            VideoSetupViewModel videoSetupViewModel = new VideoSetupViewModel
            {
                Video = new System.Collections.ObjectModel.ObservableCollection<Video>(videos),
                VideoSeries = new System.Collections.ObjectModel.ObservableCollection<VideoSeries>(series),
                Genre = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(genres),
                Format = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(formats),
                Type = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(types),
                Langauge = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(languages)
            };
            return videoSetupViewModel;
        }

        public async Task<Video> GetVideoAsync(int id)
        {
            return await _VideoService.GetVideoAsync(id);
        }

        public async Task<bool> SaveVideoAsync(Video item, string newSeries)
        {
            return await _VideoService.SaveVideoAsync(item, newSeries);
        }

        public async Task<int> DeleteVideoAsync(Video item)
        {
            return await _VideoService.DeleteVideoAsync(item);
        }

        public async Task<VideoSeries> GetVideoSeriesAsync(int id)
        {
            return await _VideoService.GetVideoSeriesAsync(id);
        }

        public async Task<List<VideoSeries>> GetAllVideoSeriesAsync()
        {
            return await _VideoService.GetAllVideoSeriesAsync();
        }

        public async Task<int> SaveVideoSeriesAsync(VideoSeries item)
        {
            await MediaItemDatabase.Init();
            if (item.Id != 0)
                return await MediaItemDatabase.database.UpdateAsync(item);
            else
                return await MediaItemDatabase.database.InsertAsync(item);
        }

        public async Task<int> DeleteVideoSeriesAsync(VideoSeries item)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.DeleteAsync(item);
        }

        public async Task<VideoSetupViewModel> GetVideoSetup()
        {
            var series = await _VideoService.GetAllVideoSeriesAsync();
            var genres = _SharedService.GetGenres();
            var formats = _VideoService.GetVideoFormats();
            var types = _VideoService.GetVideoTypes();
            var languages = _SharedService.GetLanguages();

            VideoSetupViewModel setup = new VideoSetupViewModel
            {
                VideoSeries = new System.Collections.ObjectModel.ObservableCollection<VideoSeries>(series),
                Genre = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(genres),
                Format = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(formats),
                Type = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(types),
                Langauge = new System.Collections.ObjectModel.ObservableCollection<TextValuePair<string, int>>(languages),
                Video = new System.Collections.ObjectModel.ObservableCollection<Video>(await _VideoService.GetVideosAsync())
            };
            return setup;
        }

        public async Task<Video> GetVideo(int id)
        {
            if (id <= 0) return null;
            return await _VideoService.GetVideoAsync(id);
        }
    }
}

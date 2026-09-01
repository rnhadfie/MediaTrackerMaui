using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using MauiApp1.BackEnd.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    public class VideoSetupViewModel
    {
        public ObservableCollection<VideoItem> VideoSeries { get; set; }
        public ObservableCollection<TextValuePair<string, int>> Genre { get; set; }
        public ObservableCollection<TextValuePair<string, int>> Format { get; set; }
        public ObservableCollection<TextValuePair<string, int>> Type { get; set; }

        public ObservableCollection<TextValuePair<string, int>> Langauge { get; set; }


        public ObservableCollection<Video> Video { get; set; }

        public VideoSetupViewModel()
        {
            VideoSeries = new ObservableCollection<VideoItem>();
            Genre = new ObservableCollection<TextValuePair<string, int>>();
            Format = new ObservableCollection<TextValuePair<string, int>>();
            Type = new ObservableCollection<TextValuePair<string, int>>();
            Langauge = new ObservableCollection<TextValuePair<string, int>>();
            Video = new ObservableCollection<Video>();
        }
    }
}

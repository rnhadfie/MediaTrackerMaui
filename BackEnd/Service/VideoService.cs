using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Repository;
using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Front.Components.Video;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    public class VideoService
    {
        private Lazy<VideoRepository> VideoRepository;
        private VideoRepository _VideoRepository;

        public VideoService(DataContext dataContext)
        {
            _VideoRepository = new Lazy<VideoRepository>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new VideoRepository(dataContext);
            }).Value;
        }

        public bool AddVideo(Video newVideo)
        {
            byte[] CompressedImageData = newVideo.Cover ?? [];
            if (CompressedImageData.Length > 0)
            {
                newVideo.Cover = SharedService.CompressImage(CompressedImageData, 100, 60);
            }
            return _VideoRepository.SaveVideo(newVideo);
        }

        public Video GetVideoInfo(int id)
        {
            return _VideoRepository.GetVideo(id);
        }

        public List<DisplayViewItem> GetVideos(int take)
        {
            List<Video> videoList = _VideoRepository.GetAllVideo();
            List<DisplayViewItem> videoDisplayList = new List<DisplayViewItem>();
            videoList.ForEach(x =>
            {
                videoDisplayList.Add(new DisplayViewItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Cover = x.Cover,
                    Type = MediaDataType.Video

                });
            });
            return videoDisplayList;
        }

        public List<TextValuePair<int>> GetVideoFormat()
        { 
            List<TextValuePair<int>> formats =
            [
                new TextValuePair<int>("DVD", (int)VideoFormat.DVD),
                new TextValuePair<int>("BluRay", (int)VideoFormat.Bluray),
                new TextValuePair<int>("UltraHD", (int)VideoFormat.ultraHd),
            ];

            return formats;
        }

        public List<TextValuePair<int>> GetVideoType()
        {
            List<TextValuePair<int>> formats =
            [
                new TextValuePair<int>("Movie", (int)VideoType.Movie),
                new TextValuePair<int>("Show", (int)VideoType.tvShow),
            ];

            return formats;
        }

        public List<TextValuePair<int>> GetVideoGroups()
        {

            List<TextValuePair<int>> groups = [
                new TextValuePair<int>("Live Action", 1),
                new TextValuePair<int>("Anime", 2),
                 new TextValuePair<int>("Concert", 1),
                ];
            return groups;
        }
    }


}

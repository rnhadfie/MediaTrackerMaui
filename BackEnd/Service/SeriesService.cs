using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Repository;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    public class SeriesService
    {
        private Lazy<SeriesRepository> SeriesRepository;
        private SeriesRepository _SeriesRepository;

        public SeriesService(DataContext dataContext)
        {
            _SeriesRepository = new Lazy<SeriesRepository>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new SeriesRepository(dataContext);
            }).Value;
        }

        public List<TextValuePair<int>> GetListOfSeries()
        {
            List<Series> series =  _SeriesRepository.GetSeriesList();
            List<TextValuePair<int>> listOfSeries = new List<TextValuePair<int>>();
            listOfSeries.Add(new TextValuePair<int>("None", 0));
            if (series != null || series.Count > 0)
            {
                foreach (Series s in series)
                {
                    listOfSeries.Add(new TextValuePair<int>(s.Title, s.SeriesId));
                }
            }

            return listOfSeries;
        }

        public bool AddNewSeries(Series series)
        {
            return _SeriesRepository.SaveSeriesAsync(series);
        }

        public Series GetSeries(int id)
        {
            return _SeriesRepository.GetSeriesAsync(id);
        }

        public List<DisplayViewItem> GetDisplaySeriesList(int take)
        {
            List<Series> seriesList = _SeriesRepository.GetSeriesList(take);
            List<DisplayViewItem> seriesDisplayList = new List<DisplayViewItem>();
            seriesList.ForEach(x =>
            {
                seriesDisplayList.Add(new DisplayViewItem()
                {
                    Id = x.SeriesId,
                    Name = x.Title,
                    Cover = [],
                    Type = MediaDataType.Series

                });
            });
            return seriesDisplayList;
        }

        public List<Series> GetSeriesList()
        {
            return _SeriesRepository.GetSeriesList();
        }

        public List<DisplayViewItem> GetDisplaySeriesList()
        {
            List<Series> seriesList = _SeriesRepository.GetSeriesList();
            List<DisplayViewItem> seriesDisplayList = new List<DisplayViewItem>();
            seriesList.ForEach(x =>
            {
                seriesDisplayList.Add(new DisplayViewItem()
                {
                    Id = x.SeriesId,
                    Name = x.Title,
                    Cover = [],
                    Type = MediaDataType.Series

                });
            });
            return seriesDisplayList;
        }

        public List<DisplayViewItem> GetSeriesItems(int seriesId)
        {
            Series currentSeries = _SeriesRepository.GetSeriesAsync(seriesId);

            switch (currentSeries.type)
            {
                case MediaDataType.All:
                    List<Book> books = _SeriesRepository.GetBooksInSeries(seriesId);
                    List<Video> video = _SeriesRepository.GetVideosInSeries(seriesId);
                    var displayList = books.Select(x =>
                        new DisplayViewItem()
                        {
                            Id = x.SeriesId,
                            Name = x.Title,
                            Cover = x.Cover,
                            Type = MediaDataType.Video
                        }
                    ).ToList();
                    video = _SeriesRepository.GetVideosInSeries(seriesId);
                    displayList.AddRange(video.Select(x =>
                        new DisplayViewItem()
                        {
                            Id = x.SeriesId,
                            Name = x.Name,
                            Cover = x.Cover,
                            Type = MediaDataType.Video
                        }).ToList());
                    return displayList;
                case MediaDataType.Book:
                    books = _SeriesRepository.GetBooksInSeries(seriesId);
    
                    return books.Select(x =>
                        new DisplayViewItem()
                        {
                            Id = x.SeriesId,
                            Name = x.Title,
                            Cover = x.Cover,
                            Type = MediaDataType.Video
                        }
                    ).ToList();
                case MediaDataType.Video:
                    video = _SeriesRepository.GetVideosInSeries(seriesId);
                    return video.Select(x =>
                        new DisplayViewItem()
                        {
                            Id = x.SeriesId,
                            Name = x.Name,
                            Cover = x.Cover,
                            Type = MediaDataType.Video
                        }
                    ).ToList();
            }
            return new List<DisplayViewItem>();
        }

        public List<TextValuePair<int>> GetCollectionStatus()
        {
            List<TextValuePair<int>> collectionStatusList = new List<TextValuePair<int>>(){
                new TextValuePair<int>("All", (int)CollectionStatus.All),
                new TextValuePair<int>("Not collecting",(int)CollectionStatus.NotCompleting),
                new TextValuePair<int>("Collecting", (int)CollectionStatus.Collecting),
                new TextValuePair<int>("Completed", (int)CollectionStatus.Completed),
                new TextValuePair<int>("On hold", (int)CollectionStatus.OnHold),
            };
            return collectionStatusList;
        }

        public List<TextValuePair<int>> GetMediaTypesStatus()
        {
            List<TextValuePair<int>> list = new List<TextValuePair<int>>(){
                new TextValuePair<int>("All",(int)MediaDataType.All),
                new TextValuePair<int>("Book", (int)MediaDataType.Book),
                new TextValuePair<int>("Video", (int)MediaDataType.Video),
            };
            return list;
        }
    }
}

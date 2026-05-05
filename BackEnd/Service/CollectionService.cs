using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Modals;
using MauiApp1.Repository;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    public class SeriesService
    {
        private Lazy<CollectionRepository> SeriesRepository;
        private CollectionRepository _SeriesRepository;

        public SeriesService(DataContext dataContext)
        {
            _SeriesRepository = new Lazy<CollectionRepository>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new CollectionRepository(dataContext);
            }).Value;
        }

        public List<TextValuePair<int>> GetListOfSeries(MediaDataType type = MediaDataType.All)
        {
            List<Collection> series =  _SeriesRepository.GetCollections();
            List<TextValuePair<int>> listOfSeries = new List<TextValuePair<int>>();
            listOfSeries.Add(new TextValuePair<int>("None", 0));
            if (series != null || series.Count > 0)
            {
                foreach (Collection s in series)
                {
                    if (type != MediaDataType.All && s.type != type)
                    {
                        continue;
                    }
                    listOfSeries.Add(new TextValuePair<int>(s.Title, s.SeriesId));
                }
            }

            return listOfSeries;
        }

        public bool AddNewSeries(Collection series)
        {
            return _SeriesRepository.SaveCollection(series);
        }

        public Collection GetSeries(int id)
        {
            return _SeriesRepository.GetCollection(id);
        }

        public List<DisplayViewItem> GetDisplaySeriesList(int take)
        {
            List<Collection> seriesList = _SeriesRepository.GetCollections(take);
            List<DisplayViewItem> seriesDisplayList = new List<DisplayViewItem>();
            seriesList.ForEach(x =>
            {
                seriesDisplayList.Add(new DisplayViewItem()
                {
                    Id = x.SeriesId,
                    Name = x.Title,
                    Cover = [],
                    Type = MediaDataType.Collection

                });
            });
            return seriesDisplayList;
        }

        public List<Collection> GetSeriesList()
        {
            return _SeriesRepository.GetCollections();
        }

        public List<DisplayViewItem> GetDisplaySeriesList()
        {
            List<Collection> seriesList = _SeriesRepository.GetCollections();
            List<DisplayViewItem> seriesDisplayList = new List<DisplayViewItem>();
            seriesList.ForEach(x =>
            {
                seriesDisplayList.Add(new DisplayViewItem()
                {
                    Id = x.SeriesId,
                    Name = x.Title,
                    Cover = [],
                    Type = MediaDataType.Collection

                });
            });
            return seriesDisplayList;
        }

        public List<DisplayViewItem> GetSeriesItems(int seriesId)
        {
            Collection currentSeries = _SeriesRepository.GetCollection(seriesId);

            switch (currentSeries.type)
            {
                case MediaDataType.Book:

                    List<Book> books = _SeriesRepository.GetBooksInSeries(seriesId);
    
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
                    List<Video> video = _SeriesRepository.GetVideosInSeries(seriesId);
                    return video.Select(x =>
                        new DisplayViewItem()
                        {
                            Id = x.SeriesId,
                            Name = x.Name,
                            Cover = x.Cover,
                            Type = MediaDataType.Video
                        }
                    ).ToList();

                default:
                    books = _SeriesRepository.GetBooksInSeries(seriesId);
                    video = _SeriesRepository.GetVideosInSeries(seriesId);
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

        public List<TextValuePair<int>> GetSertiesType()

        {
            List<TextValuePair<int>> SertiesTypeList = new List<TextValuePair<int>>(){
                new TextValuePair<int>("All", (int)MediaDataType.All),
                new TextValuePair<int>("Book",(int)MediaDataType.Book),
                new TextValuePair<int>("Video", (int)MediaDataType.Video),
                new TextValuePair<int>("Cd", (int)MediaDataType.Cd),
            };
            return SertiesTypeList;
        }


    }
}

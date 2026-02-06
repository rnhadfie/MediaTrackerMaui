using MauiApp1.BackEnd.Database;
using MauiApp1.Repository;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using System;
using System.Collections.Generic;
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

        public async Task<List<TextValuePair<int>>> GetListOfSeries()
        {
            List<Series> series = await _SeriesRepository.GetSeriessAsync();
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

        public List<TextValuePair<int>> GetCollectionStatus()
        {
            List<TextValuePair<int>> collectionStatusList = new List<TextValuePair<int>>(){
                new TextValuePair<int>("Not collecting",(int)CollectionStatus.NotCompleting),
                new TextValuePair<int>("Collecting", (int)CollectionStatus.Collecting),
                new TextValuePair<int>("Completed", (int)CollectionStatus.Completed),
                new TextValuePair<int>("On hold", (int)CollectionStatus.OnHold),
            };
            return collectionStatusList;
        }
    }
}

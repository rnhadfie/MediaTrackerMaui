using MauiApp1.BackEnd.Modals;
using MauiApp1.Modals;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Interface
{
    public interface ICollectionRepository
    {
        public List<Collection> GetCollections();

        public List<Collection> GetCollections(int take);


        public Collection GetCollection(int id);

        public bool SaveCollection(Collection item);

        public List<Book> GetBooksInSeries(int seriesId);

        public List<Video> GetVideosInSeries(int seriesId);

        public List<Cd> GetCdsInSeries(int seriesId);

        public List<Other> GetOtherInSeries(int seriesId);

        public bool DeleteCollection(Collection item);


    }
}

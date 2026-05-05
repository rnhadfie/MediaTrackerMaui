
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Shared;
using MauiApp1.BackEnd.Interface;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Modals;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace MauiApp1.Repository
{
    public class CollectionRepository: ICollectionRepository
    {
        DataContext _dbContext;

        public CollectionRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }
        public List<Collection> GetCollections()
        {
            return CacheManager.GetOrAdd("collections:all", () => new ObservableCollection<Collection>(_dbContext.SeriesTable).ToList());
        }

        public List<Collection> GetCollections(int take)
        {
            return CacheManager.GetOrAdd($"collections:all:take:{take}", () => new ObservableCollection<Collection>(_dbContext.SeriesTable.Take(take)).ToList());
        }

        public Collection GetCollection(int id)
        {
            try
            {
                var context = _dbContext;
            var key = $"collections:id:{id}";
                return CacheManager.GetOrAdd(key, () =>
                {
                    var result = context.SeriesTable.Where(x => x.SeriesId == id);
                    return result.FirstOrDefault<Collection>();
                });
            }
            catch (Exception ex)
            {
                return new Collection() { Title = "", SeriesId = -1};
            }
        }

        public bool SaveCollection(Collection item)
        {
            try
            {
                var context = _dbContext;
                if (item.SeriesId != -1)
                    context.SeriesTable.Update(item);
                else
                {
                    int maxId = 1;
                    if (context.SeriesTable.Any())
                    {
                        maxId = context.SeriesTable.Max(x => x.SeriesId);
                    }
                    item.SeriesId = maxId + 1;

                    context.SeriesTable.Add(item);

                }
                int result = context.SaveChanges();
                CacheManager.RemovePrefix("collections:");
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Book> GetBooksInSeries(int seriesId)
        {
            return CacheManager.GetOrAdd($"collections:{seriesId}:books", () => new ObservableCollection<Book>(_dbContext.BookTable.Where(x=>x.SeriesId == seriesId)).ToList());
        }

        public List<Video> GetVideosInSeries(int seriesId)
        {
            return CacheManager.GetOrAdd($"collections:{seriesId}:videos", () => new ObservableCollection<Video>(_dbContext.VideoTable.Where(x => x.SeriesId == seriesId)).ToList());
        }

        public List<Cd> GetCdsInSeries(int seriesId)
        {
            return CacheManager.GetOrAdd($"collections:{seriesId}:cds", () => new ObservableCollection<Cd>(_dbContext.MusicTable.Where(x => x.Collection == seriesId)).ToList());
        }

        public List<Other> GetOtherInSeries(int seriesId)
        {
            return CacheManager.GetOrAdd($"collections:{seriesId}:other", () => new ObservableCollection<Other>(_dbContext.OtherTable.Where(x => x.Collection == seriesId)).ToList());
        }


        public bool DeleteCollection(Collection item)
        {
            try
            {
                _dbContext.SeriesTable.Remove(item);
                CacheManager.RemovePrefix("collections:");
                return true;
            } catch (Exception ex) 
            {
                return false;
            }
        }
    }
}

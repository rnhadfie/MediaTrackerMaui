using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Shared;
using MauiApp1.BackEnd.Interface;
using MauiApp1.Modals;
using System.Collections.ObjectModel;

namespace MauiApp1.BackEnd.Repository
{
    public class MusicRepository: IMusicRepository
    {
        DataContext _dbContext;

        public MusicRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }
        public List<Cd> GetCdList()
        {
            return CacheManager.GetOrAdd("music:all", () => new ObservableCollection<Cd>(_dbContext.MusicTable).ToList());
        }

        public List<Cd> GetCdList(int take)
        {
            return CacheManager.GetOrAdd($"music:all:take:{take}", () => new ObservableCollection<Cd>(_dbContext.MusicTable.Take(take)).ToList());
        }

        public Cd GetCd(int id)
        {
            try
            {
                var key = $"music:id:{id}";
                return CacheManager.GetOrAdd(key, () =>
                {
                    var context = _dbContext;
                    var result = context.MusicTable.Where(x => x.Collection == id);
                    return result.FirstOrDefault<Cd>();
                });
            }
            catch (Exception ex)
            {
                return new Cd() { Name = "", Collection = -1 };
            }
        }

        public bool SaveCd(Cd item)
        {
            try
            {
                var context = _dbContext;
                if (item.Collection != -1)
                    context.MusicTable.Update(item);
                else
                {
                    int maxId = 1;
                    if (context.SeriesTable.Any())
                    {
                        maxId = context.MusicTable.Max(x => x.Collection);
                    }
                    item.Collection = maxId + 1;

                    context.MusicTable.Add(item);

                }
                int result = context.SaveChanges();
                CacheManager.RemovePrefix("music:");
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool DeleteCd(Cd item)
        {
            try
            {
                _dbContext.MusicTable.Remove(item);
                CacheManager.RemovePrefix("music:");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

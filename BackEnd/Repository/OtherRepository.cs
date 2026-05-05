using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Shared;
using MauiApp1.BackEnd.Interface;
using MauiApp1.BackEnd.Modals;
using System.Collections.ObjectModel;

namespace MauiApp1.BackEnd.Repository
{
    public class OtherRepository : IOtherRepository
    {
        DataContext _dbContext;

        public OtherRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }
        public List<BackEnd.Modals.Other> GetOtherList()
        {
            return CacheManager.GetOrAdd("other:all", () => new ObservableCollection<BackEnd.Modals.Other>(_dbContext.OtherTable).ToList());
        }

        public List<BackEnd.Modals.Other> GetOtherList(int take)
        {
            return CacheManager.GetOrAdd($"other:all:take:{take}", () => new ObservableCollection<BackEnd.Modals.Other>(_dbContext.OtherTable.Take(take)).ToList());
        }

        public BackEnd.Modals.Other GetOtherItem(int id)
        {
            try
            {
                var key = $"other:id:{id}";
                return CacheManager.GetOrAdd(key, () =>
                {
                    var context = _dbContext;
                    var result = context.OtherTable.Where(x => x.Collection == id);
                    return result.FirstOrDefault<BackEnd.Modals.Other>();
                });
            }
            catch (Exception ex)
            {
                return new BackEnd.Modals.Other() { Name = "", Collection = -1 };
            }
        }

        public bool SaveOtherItem(BackEnd.Modals.Other item)
        {
            try
            {
                var context = _dbContext;
                if (item.Collection != -1)
                    context.OtherTable.Update(item);
                else
                {
                    int maxId = 1;
                    if (context.SeriesTable.Any())
                    {
                        maxId = context.OtherTable.Max(x => x.Collection);
                    }
                    item.Collection = maxId + 1;

                    context.OtherTable.Add(item);

                }
                int result = context.SaveChanges();
                CacheManager.RemovePrefix("other:");
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool DeleteOtherItem(BackEnd.Modals.Other item)
        {
            try
            {
                _dbContext.OtherTable.Remove(item);
                CacheManager.RemovePrefix("other:");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

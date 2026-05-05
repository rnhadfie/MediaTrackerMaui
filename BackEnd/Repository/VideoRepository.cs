using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Modals;
using System.Collections.ObjectModel;

namespace MauiApp1.BackEnd.Repository
{
    public class VideoRepository
    {
        DataContext _dbContext;

        public VideoRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        public List<Video> GetAllVideo()
        {
            return CacheManager.GetOrAdd("videos:all", () => new ObservableCollection<Video>(_dbContext.VideoTable).ToList());
        }

        public List<Video> GetAllVideo(int take)
        {
            return CacheManager.GetOrAdd($"videos:all:take:{take}", () => new ObservableCollection<Video>(_dbContext.VideoTable.Take(take)).ToList());
        }

        public List<Video> GetAllVideo(VideoFitler filter)
        {
            var table = _dbContext.VideoTable;
            IQueryable<Video> fitleredTable = null;
            if (filter.Type > 0)
            {
                fitleredTable = table.Where(x => x.Type == filter.Type);
            }
            /*if (filter.Genre != null)
            {
                fitleredTable = table.Where(x => x.Genre == filter.Genre);
            }*/
            if (filter.Series != null)
            {
                fitleredTable = table.Where(x => x.SeriesId == filter.Series);
            }
            if (filter.Group != null)
            {
                fitleredTable = table.Where(x => x.Category == filter.Group);
            }
            if (filter.Take != null)
            {
                fitleredTable = table.Take(filter.Take.Value);
            }
            if (fitleredTable != null)
            {
                var key = $"videos:filter:{filter.Type}:{filter.Series}:{filter.Group}:{filter.Take}";
                return CacheManager.GetOrAdd(key, () => new ObservableCollection<Video>(fitleredTable).ToList());
            }

            return CacheManager.GetOrAdd("videos:all", () => new ObservableCollection<Video>(table).ToList());
        }

        public Video GetVideo(int id)
        {
            try
            {
                var key = $"videos:id:{id}";
                return CacheManager.GetOrAdd(key, () =>
                {
                    var context = _dbContext;
                    var result = context.VideoTable.Where(x => x.Id == id);
                    return result.FirstOrDefault<Video>();
                });
            }
            catch (Exception ex)
            {
                return new Video() { Name = "" };
            }
        }

        public bool SaveVideo(Video item)
        {
            try
            {
                var context = _dbContext;
                if (item.Id > 0)
                    context.VideoTable.Update(item);
                else
                {
                    int maxId = 1;
                    if (context.VideoTable.Any())
                    {
                        maxId = context.VideoTable.Max(x => x.Id);
                    }
                    item.Id = maxId + 1;

                    context.VideoTable.Add(item);


                }
                int result = context.SaveChanges();
                CacheManager.RemovePrefix("videos:");
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteVideo(Video item)
        {
            try
            {
                _dbContext.VideoTable.Remove(item);
                CacheManager.RemovePrefix("videos:");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

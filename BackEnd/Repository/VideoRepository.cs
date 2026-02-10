using MauiApp1.BackEnd.Database;
using MauiApp1.Service.Modals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
            return new ObservableCollection<Video>(_dbContext.VideoTable).ToList();
        }

        public List<Video> GetAllVideo(int take)
        {
            return new ObservableCollection<Video>(_dbContext.VideoTable.Take(take)).ToList();
        }

        public Video GetVideo(int id)
        {
            try
            {
                var context = _dbContext;
                var result = context.VideoTable.Where(x => x.Id == id);
                return result.FirstOrDefault<Video>();
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
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

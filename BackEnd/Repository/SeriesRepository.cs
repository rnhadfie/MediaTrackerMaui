
using MauiApp1.BackEnd.Database;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MauiApp1.Repository
{
    public class SeriesRepository
    {
        DataContext _dbContext;

        public SeriesRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }
        public List<Series> GetSeriesList()
        {
            return new ObservableCollection<Series>(_dbContext.SeriesTable).ToList();
        }

        public List<Series> GetSeriesList(int take)
        {
            return new ObservableCollection<Series>(_dbContext.SeriesTable.Take(take)).ToList();
        }

        public Series GetSeriesAsync(int id)
        {
            try
            {
                var context = _dbContext;
            var result = context.SeriesTable.Where(x => x.SeriesId == id);
                return result.FirstOrDefault<Series>();
            }
            catch (Exception ex)
            {
                return new Series() { Title = "", SeriesId = -1};
            }
        }

        public bool SaveSeriesAsync(Series item)
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
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /*
        public async Task<int> DeleteSeriesAsync(Series item)
        {
            await databaseService.Init();
            return await databaseService.Database.DeleteAsync(item);
        }*/
    }
}

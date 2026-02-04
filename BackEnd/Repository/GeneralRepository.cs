using MauiApp1.BackEnd.Database;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Repository
{
    public class SeriesRepository
    {
        DatabaseService databaseService;
        SQLiteAsyncConnection database;

        public SeriesRepository()
        {
            databaseService = new DatabaseService();
            database = databaseService.Database;
        }
        public async Task<List<Series>> GetSeriessAsync()
        {

            await databaseService.Init();
            return await databaseService.Database.Table<Series>().ToListAsync();
        }

        public async Task<Series> GetSeriesAsync(int id)
        {
            await databaseService.Init();
            return await databaseService.Database.Table<Series>().Where(i => i.SeriesId == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveSeriesAsync(Series item)
        {
            await databaseService.Init();
            if (item.SeriesId != 0)
                return await databaseService.Database.UpdateAsync(item);
            else
                return await databaseService.Database.InsertAsync(item);
        }

        public async Task<int> DeleteSeriesAsync(Series item)
        {
            await databaseService.Init();
            return await databaseService.Database.DeleteAsync(item);
        }
    }
}

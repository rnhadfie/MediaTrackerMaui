using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Interface;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Repository
{
    public class VideoRepository: IVideoRepository
    {
        public async Task<List<Video>> GetVideosAsync()
        {
            try
            {
                await MediaItemDatabase.Init();

                return await MediaItemDatabase.database.Table<Video>().ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Video>();
            }
        }

        public async Task<List<Video>> GetVideosNotReadAsync()
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.Table<Video>().Where(t => !t.Watched).ToListAsync();
        }

        public async Task<Video> GetVideoAsync(int id)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.Table<Video>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveVideoAsync(Video item)
        {
            await MediaItemDatabase.Init();
            if (item.Id != 0)
                return await MediaItemDatabase.database.UpdateAsync(item);
            else
                item.Id = null;
            return await MediaItemDatabase.database.InsertAsync(item);
        }

        public async Task<int> DeleteVideoAsync(Video item)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.DeleteAsync(item);
        }

      
        public async Task<List<VideoSeries>> GetAllVideoSeriesAsync()
        {
            try
            {
                await MediaItemDatabase.Init();
                return await MediaItemDatabase.database.Table<VideoSeries>().ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<VideoSeries>();
            }
        }
        public async Task<VideoSeries> GetVideoSeriesAsync(int id)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.Table<VideoSeries>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveVideoSeriesAsync(VideoSeries item)
        {
            try
            {
                await MediaItemDatabase.Init();
                if (item.Id != 0)
                {
                    await MediaItemDatabase.database.UpdateAsync(item);
                    return item.Id;
                }
                else
                {
                    await MediaItemDatabase.database.InsertAsync(item);
                    return item.Id;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> DeleteVideoSeriesAsync(VideoSeries item)
        {
            await MediaItemDatabase.Init();
            return await MediaItemDatabase.database.DeleteAsync(item);
        }

        public async Task<bool> SaveBooksAsync(List<Book> books)
        {
            try
            {
                await MediaItemDatabase.Init();
                var db = MediaItemDatabase.database;
                // Use InsertOrReplace to avoid duplicates (requires primary key)
                foreach (var b in books)
                {
                    await db.InsertOrReplaceAsync(b);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SaveVideoSeriesAsync(List<VideoSeries> series)
        {
            try
            {
                await MediaItemDatabase.Init();
                var db = MediaItemDatabase.database;
                foreach (var s in series)
                {
                    await db.InsertOrReplaceAsync(s);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Task<bool> SaveVideosAsync(List<Video> videos)
        {
            throw new NotImplementedException();
        }
    }
}

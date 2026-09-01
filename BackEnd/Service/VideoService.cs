using ClosedXML.Excel;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using MauiApp1.BackEnd.Repository;
using MauiApp1.BackEnd.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    internal class VideoService
    {
        private Lazy<VideoRepository> VideoRepository;
        private VideoRepository _VideoRepository;

        public VideoService()
        {
            VideoRepository = new Lazy<VideoRepository>(() => new VideoRepository());
            _VideoRepository = VideoRepository.Value;
        }

        public async Task<List<Video>> GetVideosAsync()
        {
            List<Video> videos = await _VideoRepository.GetVideosAsync();
           
            return videos;
        }

        public async Task<List<Video>> GetVideosNotReadAsync()
        {
            List<Video> videos = await _VideoRepository.GetVideosNotReadAsync();
         
            return videos;
        }

        public async Task<Video> GetVideoAsync(int id)
        {
            Video video = await _VideoRepository.GetVideoAsync(id);
            return video;
        }

        public async Task<VideoItem> GetVideoSeriesAsync(int id)
        {
            VideoItem videoSeries = await _VideoRepository.GetVideoSeriesAsync(id);
            return videoSeries;
        }

        public async Task<bool> SaveVideoAsync(Video item, string newSeries)
        {
            /*
            bool result = true;
            try
            {
                if (!string.IsNullOrWhiteSpace(newSeries) && item.Series <= 0)
                {
                    VideoItem videoSeries = new VideoItem
                    {
                        Title = newSeries,

                    };
                    int id = await _VideoRepository.SaveVideoSeriesAsync(videoSeries);
                    item.Series = id;

                    result = result && id > 0;

                }
                return result && await _VideoRepository.SaveVideoAsync(item) > 0;
            }
            catch (Exception ex)
            {
                return false;
            } */
            return false;

        }

        public async Task<int> DeleteVideoAsync(Video item)
        {

            return await _VideoRepository.DeleteVideoAsync(item);
        }


        public async Task<List<VideoItem>> GetAllVideoSeriesAsync()
        {
            return await _VideoRepository.GetAllVideoSeriesAsync();
        }

        public async Task<int> SaveVideoSeriesAsync(VideoItem item)
        {
            return await _VideoRepository.SaveVideoSeriesAsync(item);
        }
        public async Task<int> DeleteVideoSeriesAsync(VideoItem item)
        {
            return await _VideoRepository.DeleteVideoSeriesAsync(item);
        }

        public List<TextValuePair<string, int>> GetVideoFormats()
        {
            return Enum.GetValues(typeof(VideoFormat)).Cast<VideoFormat>().Select(g => new TextValuePair<string, int>(g.ToString(), (int)g)).ToList();
        }

        public List<TextValuePair<string, int>> GetVideoTypes()
        {
            return Enum.GetValues(typeof(VideoType)).Cast<VideoType>().Select(g => new TextValuePair<string, int>(g.ToString(), (int)g)).ToList();
        }

        public List<TextValuePair<string, int>> GetVideoTags()
        {
            return Enum.GetValues(typeof(VideoTag)).Cast<VideoTag>().Select(g => new TextValuePair<string, int>(g.ToString(), (int)g)).ToList();
        }

        public async Task<XLWorkbook> ExportVideoData(XLWorkbook workBook)
        {
            List<Video> videos = await _VideoRepository.GetVideosAsync();
            List<VideoItem> videoSeries = await _VideoRepository.GetAllVideoSeriesAsync();

            var videoSheet = workBook.AddWorksheet("Videos");
            videoSheet.Cell(1, 1).InsertData(videos, true);

            var seriesSheet = workBook.AddWorksheet("VideoSeries");
            seriesSheet.Cell(1, 1).InsertData(videoSeries, true);

            return workBook;
        }

        /*

        public async Task<bool> ImportVideoDataFromExcel(XLWorkbook wb)
        {
            try
            {
                var videoDtos = ReadVideosFromExcel(wb);
                var videos = videoDtos.Select(dto => MapDtoToVideo(dto)).ToList();
                bool success = await _VideoRepository.SaveVideosAsync(videos);

                var videoSeries = ReadVideoSeriesFromExcel(wb);
                success = success && await _VideoRepository.SaveVideoSeriesAsync(videoSeries);

                var publishers = ReadPublishersFromExcel(wb);
                success = success && await _VideoRepository.SavePublishersAsync(publishers);

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing videos from Excel: {ex.Message}");
                return false;
            }
        } */

        public List<VideoExcel> ReadVideosFromExcel(XLWorkbook wb)
        {
            var ws = wb.Worksheet(1);
            var headerRow = ws.Row(1);
            var headers = headerRow.CellsUsed()
                .Select((c, i) => new { Name = c.GetString().Trim(), Index = i + 1 })
                .ToDictionary(x => x.Name, x => x.Index);

            var list = new List<VideoExcel>();
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var dto = new VideoExcel    
                {
                    Id = headers.ContainsKey("Id") ? row.Cell(headers["Id"]).GetString() : null,
                    Name = headers.ContainsKey("Title") ? row.Cell(headers["Title"]).GetString() : null,
                    Genre = headers.ContainsKey("Genre") ? row.Cell(headers["Genre"]).GetString() : null,
                    Format = headers.ContainsKey("Format") ? row.Cell(headers["Format"]).GetString() : null,
                    Season = headers.ContainsKey("Season") ? row.Cell(headers["Season"]).GetString() : null,
                    Tag = headers.ContainsKey("Read") ? row.Cell(headers["Read"]).GetString() : null,
                    Language = headers.ContainsKey("Language") ? row.Cell(headers["Language"]).GetString() : null,
                    SeriesId = headers.ContainsKey("SeriesId") ? row.Cell(headers["SeriesId"]).GetString() : null,
                    Type = headers.ContainsKey("Type") ? row.Cell(headers["Type"]).GetString() : null
                };
                list.Add(dto);
            }
            return list;
        } 
        /*
        public List<VideoItem> ReadVideoSeriesFromExcel(XLWorkbook wb)
        {
            var ws = wb.Worksheet(2);
            var headerRow = ws.Row(1);
            var headers = headerRow.CellsUsed()
                .Select((c, i) => new { Name = c.GetString().Trim(), Index = i + 1 })
                .ToDictionary(x => x.Name, x => x.Index);

            var list = new List<VideoItem>();
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var dto = new VideoItem
                {
                    Id = headers.ContainsKey("Id") ? int.TryParse(row.Cell(headers["Id"]).GetString(), out var id) ? id : 0 : 0,
                    Title = headers.ContainsKey("Title") ? row.Cell(headers["Title"]).GetString() : null,
                    Ongoing = headers.ContainsKey("Ongoing") ? row.Cell(headers["Ongoing"]).GetString() == "Y" : false,
                    Collecting = headers.ContainsKey("Collecting") ? row.Cell(headers["Collecting"]).GetString() : null,
                    UpToDateComplete = headers.ContainsKey("UpToDateComplete") ? row.Cell(headers["UpToDateComplete"]).GetString() == "Y" : false,
                    Parent = headers.ContainsKey("Parent") ? row.Cell(headers["Parent"]).GetString() : null
                };
                list.Add(dto);
            }
            return list;
        }
        /*
        public Book MapDtoToBook(BookExcelDto dto)
        {

            var genres = dto.Genre.Split(",");
            List<int> bookGenre = new List<int>();
            foreach (var item in genres)
            {
                bookGenre.Add((int.TryParse(item, out var gen) ? gen : 0));
            }

            var volumes = dto.Volume.Split(",");
            List<int> bookVolumes = new List<int>();
            foreach (var item in volumes)
            {
                bookVolumes.Add(int.TryParse(item, out var gen) ? gen : 0);
            }
            var book = new BookDT
            {
                Id = (int.TryParse(dto.Id, out var id) ? id : default),
                Title = dto.Title,
                Author = string.IsNullOrWhiteSpace(dto.Author) ? null : dto.Author,
                Artist = string.IsNullOrWhiteSpace(dto.Artist) ? null : dto.Artist,
                Publisher = int.TryParse(dto.Publisher, out var p) ? p : 0,
                Genre = bookGenre,
                Format = (BookFormat)(int.TryParse(dto.Format, out var f) ? f : default),
                Volume = bookVolumes,
                Read = dto.Read == "Y",
                Language = (Language)(int.TryParse(dto.Language, out var l) ? l : 0),
                BookSeries = int.TryParse(dto.BookSeries, out var bs) ? bs : 0,
                Type = (BookType)(int.TryParse(dto.Type, out var t) ? t : default)
            };
            return book.GetBook();
        }*/
    }
}

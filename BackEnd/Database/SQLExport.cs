using ClosedXML.Excel;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Modals;
using System.Data;


namespace MauiApp1.BackEnd.Database
{
    using static MauiApp1.BackEnd.Shared.Enums;
    using Collection = MauiApp1.Modals.Collection;

    public static class SQLExport
    {
        public static void Export(DataContext dataContext, string excelFilePath)
        {
            // 1. Retrieve Data from SQLite into a DataTable
            DataTable dataTable = new DataTable();

            using (dataContext)
            {
                // 2. and 3. Create Excel Workbook and Export DataTable
                var workbook = new XLWorkbook();

                // Series
                dataTable = dataContext.SeriesTable.ToDataTable();
                dataTable.TableName = "SeriesTable";
                ExportToExcel(dataTable, ref workbook);

                // Books
                dataTable = dataContext.BookTable.ToDataTable();
                dataTable.TableName = "BookTable";
                ExportToExcel(dataTable, ref workbook);

                // Videos
                dataTable = dataContext.VideoTable.ToDataTable();
                dataTable.TableName = "VideoTable";
                ExportToExcel(dataTable, ref workbook);

                // Music
                dataTable = dataContext.MusicTable.ToDataTable();
                dataTable.TableName = "MusicTable";
                ExportToExcel(dataTable, ref workbook);

                // Other
                dataTable = dataContext.OtherTable.ToDataTable();
                dataTable.TableName = "OtherTable";
                ExportToExcel(dataTable, ref workbook);

                excelFilePath = System.IO.Path.Combine(excelFilePath, "mediaExportfile.xlsx");

                // 4. Save the File
                workbook.SaveAs(excelFilePath);
                Console.WriteLine($"Successfully exported data to {excelFilePath}");
            }
        }

        public static void Import(DataContext dataContext, XLWorkbook workbook)
        {
            try
            {
                // Helper: build header map for a worksheet
                static Dictionary<string, int> BuildHeaderMap(IXLWorksheet ws)
                {
                    var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    var headerRow = ws.FirstRowUsed();
                    if (headerRow == null) return map;
                    foreach (var cell in headerRow.CellsUsed())
                    {
                        var key = cell.GetString()?.Trim();
                        if (!string.IsNullOrEmpty(key) && !map.ContainsKey(key))
                            map[key] = cell.WorksheetColumn().ColumnNumber();
                    }
                    return map;
                }

                // Series
                {
                    var workSheet = workbook.Worksheet("SeriesTable");
                    if (workSheet != null)
                    {
                        var map = BuildHeaderMap(workSheet);
                        var headerRow = workSheet.FirstRowUsed();
                        if (headerRow != null && map.Any())
                        {
                            foreach (var row in workSheet.RowsUsed().Skip(1))
                            {
                                if (!map.ContainsKey("SeriesId")) continue;

                                var idCell = row.Cell(map["SeriesId"]);
                                if (!double.TryParse(idCell.GetString(), out var idAsDouble))
                                    continue;

                                int seriesId = (int)idAsDouble;

                                Collection? exists = dataContext.SeriesTable.FirstOrDefault(s => s.SeriesId == seriesId);


                                row.Cell(map["Title"]).TryGetValue<string>(out string series_title);
                                row.Cell(map["Author"]).TryGetValue<string>(out string series_author);
                                row.Cell(map["Publisher"]).TryGetValue<string> (out string series_publisher);
                                row.Cell(map["Artist"]).TryGetValue<string>(out string series_artist);
                                row.Cell(map["type"]).TryGetValue<int>(out int series_type);
                                row.Cell(map["CollectionStatus"]).TryGetValue<int>(out int series_status);
                                row.Cell(map["TotalVolumes"]).TryGetValue<int?>(out int? series_totalVolumes);
                                    
                                var newSeries = new Collection
                                {
                                    SeriesId = seriesId,
                                    Title = series_title,
                                    Author = series_author,
                                    Publisher = series_publisher,
                                    Artist = series_artist,
                                    type = (MediaDataType)series_type,
                                    CollectionStatus = (CollectionStatus)series_status,
                                    TotalVolumes = series_totalVolumes,
                                };

                                if (exists != null) { 
                                    exists.Title = series_title;
                                exists.Author = series_author;
                                exists.Publisher = series_publisher;
                                exists.Artist = series_artist;
                                exists.type = (MediaDataType)series_type;
                                exists.CollectionStatus = (CollectionStatus)series_status;

                                exists.TotalVolumes = series_totalVolumes;
                                    dataContext.SeriesTable.Update(exists);
                                    dataContext.SaveChanges();
                            }
                                else { 
                                    dataContext.SeriesTable.Add(newSeries);
                            }
                        }
                    }
                }

                // Book
                    var bookWorkSheet = workbook.Worksheet("BookTable");
                    if (bookWorkSheet != null)
                    {
                        var map = BuildHeaderMap(bookWorkSheet);
                        if (map.Any())
                        {
                            foreach (var row in bookWorkSheet.RowsUsed().Skip(1))
                            {
                                if (!map.ContainsKey("Id")) continue;
                                int id = row.Cell(map["Id"]).GetValue<int>();

                                row.Cell(map["SeriesId"]).TryGetValue<int>(out int book_seriesId);
                                row.Cell(map["Title"]).TryGetValue<string>(out string book_title);
                                row.Cell(map["Author"]).TryGetValue<string>(out string book_author);
                                row.Cell(map["Publisher"]).TryGetValue<string>(out string book_publisher);
                                row.Cell(map["Artist"]).TryGetValue<string>(out string book_artist);
                                row.Cell(map["type"]).TryGetValue<int>(out int book_type);
                                row.Cell(map["Genre"]).TryGetValue<int>(out int book_genre);
                                row.Cell(map["Volume"]).TryGetValue<int>(out int book_volume);
                                row.Cell(map["Format"]).TryGetValue<int>(out int book_format);
                                row.Cell(map["Cover"]).TryGetValue<byte[]?>(out byte[]? book_cover);

                                Book? exists = dataContext.BookTable.FirstOrDefault(s => s.Id == id);
                                var entity = new Book
                                {
                                    Id = id,
                                    SeriesId = book_seriesId,
                                    Title = book_title,
                                    Author = book_author,
                                    Publisher = book_publisher,
                                    Artist = book_artist,
                                    Cover = book_cover ?? Array.Empty<byte>(),
                                    Genre = book_genre,
                                    Format = book_format,
                                    Type = book_type,
                                    Volume = book_volume,
                                };

                                if (exists != null) {
                                    exists.SeriesId = book_seriesId;
                                    exists.Title = book_title;
                                    exists.Author = book_author;
                                    exists.Publisher = book_publisher;
                                    exists.Artist = book_artist;
                                    exists.Cover = book_cover ?? Array.Empty<byte>();
                                    exists.Genre = book_genre;
                                    exists.Format = book_format;
                                    exists.Type = book_type;
                                    exists.Volume = book_volume;
                                    dataContext.BookTable.Update(exists);
                                    dataContext.SaveChanges();
                                }
                                else
                                    dataContext.BookTable.Add(entity);
                            }
                        }
                    }

                // Video
                    var videoWorkSheet = workbook.Worksheet("VideoTable");
                    if (videoWorkSheet != null)
                    {
                        var map = BuildHeaderMap(videoWorkSheet);
                        if (map.Any())
                        {
                            foreach (var row in videoWorkSheet.RowsUsed().Skip(1))
                            {
                                if (!map.ContainsKey("Id")) continue;
                                int id = row.Cell(map["Id"]).GetValue<int>();
                                row.Cell(map["SeriesId"]).TryGetValue<int>(out int video_seriesId);
                                row.Cell(map["Name"]).TryGetValue<string>(out string video_name);
                                row.Cell(map["type"]).TryGetValue<int>(out int video_type);
                                row.Cell(map["Genre"]).TryGetValue<int>(out int video_genre);
                                row.Cell(map["VideoFormat"]).TryGetValue<int>(out int video_format);
                                row.Cell(map["Category"]).TryGetValue<int>(out int video_category);
                                row.Cell(map["Cover"]).TryGetValue<byte[]?>(out byte[]? video_cover);

                                Video? exists = dataContext.VideoTable.FirstOrDefault(s => s.Id == id);
                                var entity = new Video
                                {
                                    Id = id,
                                    SeriesId = video_seriesId,
                                    Cover = video_cover ?? Array.Empty<byte>(),
                                    Genre = video_genre,
                                    Type = video_type,
                                    Name = video_name,
                                    Category = video_category,
                                    VideoFormat = video_format,
                                };

                                if (exists != null)
                                {
                                    exists.SeriesId = video_seriesId;  
                                    exists.Cover = video_cover ?? Array.Empty<byte>();
                                    exists.Genre = video_genre;
                                    exists.Type = video_type;
                                    exists.Name = video_name;
                                    exists.Category = video_category;
                                    exists.VideoFormat = video_format;
                                    dataContext.VideoTable.Update(exists);
                                    dataContext.SaveChanges();
                                }
                                else
                                    dataContext.VideoTable.Add(entity);
                            }
                        }
                    }

                // Music
                    var musicWorkSheet = workbook.Worksheet("MusicTable");
                    if (musicWorkSheet != null)
                    {
                        var map = BuildHeaderMap(musicWorkSheet);
                        if (map.Any())
                        {
                            foreach (var row in musicWorkSheet.RowsUsed().Skip(1))
                            {
                                if (!map.ContainsKey("Id")) continue;
                                int id = row.Cell(map["Id"]).GetValue<int>();
                                row.Cell(map["Language"]).TryGetValue<int>(out int music_language);
                                row.Cell(map["Name"]).TryGetValue<string>(out string music_name);
                                row.Cell(map["Artist"]).TryGetValue<string>(out string music_artist);
                                row.Cell(map["Collection"]).TryGetValue<int>(out int music_Collection);
                                row.Cell(map["MusicGenre"]).TryGetValue<int>(out int music_genre);
                                row.Cell(map["Cover"]).TryGetValue<byte[]?>(out byte[]? music_cover);

                                Cd? exists = dataContext.MusicTable.FirstOrDefault(s => s.Id == id);
                                var entity = new Cd
                                {
                                    Id = id,
                                    Language = music_language,
                                    MusicGenre = music_genre,
                                    Collection = music_Collection,
                                    Artist = music_artist,
                                    Cover = music_cover ?? Array.Empty<byte>(),
                                    Name = music_name,
                                };

                                if (exists != null)
                                {
                                    exists.Language = music_language;
                                    exists.MusicGenre = music_genre;
                                    exists.Collection = music_Collection;
                                    exists.Artist = music_artist;
                                    exists.Cover = music_cover ?? Array.Empty<byte>();
                                    exists.Name = music_name;
                                    dataContext.MusicTable.Update(exists);
                                    dataContext.SaveChanges();
                                }
                                else
                                    dataContext.MusicTable.Add(entity);
                            }
                        }
                    }

                // Other
                    var otherWorkbook = workbook.Worksheet("OtherTable");
                    if (otherWorkbook != null)
                    {
                        var map = BuildHeaderMap(otherWorkbook);
                        if (map.Any())
                        {
                            foreach (var row in otherWorkbook.RowsUsed().Skip(1))
                            {
                                if (!map.ContainsKey("Id")) continue;
                                int id = row.Cell(map["Id"]).GetValue<int>();
                                row.Cell(map["Title"]).TryGetValue<string>(out string other_title);
                                row.Cell(map["Image"]).TryGetValue<byte[]?>(out byte[]? other_image);
                                row.Cell(map["Collection"]).TryGetValue<int>(out int other_collection);

                                Other? exists = dataContext.OtherTable.FirstOrDefault(s => s.Id == id);
                                var entity = new Other
                                {
                                    Id = id,
                                    Collection = other_collection,
                                    Name = other_title,
                                    Image = other_image ?? Array.Empty<byte>(),
                                };

                                if (exists != null)
                                {
                                    exists.Collection = other_collection;
                                    exists.Name = other_title;
                                    exists.Image = other_image ?? Array.Empty<byte>();

                                    dataContext.OtherTable.Update(exists);
                                    dataContext.SaveChanges();
                                }
                                else
                                    dataContext.OtherTable.Add(entity);
                            }
                        }
                    }
                }

                // Persist changes
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Surface error for debugging; don't swallow silently
                Console.WriteLine($"Import failed: {ex}");
                throw;
            }
        }

        private static void ExportToExcel(DataTable dataTable, ref XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add(dataTable.TableName);
            // Insert the DataTable data, including column headers
            worksheet.Cell(1, 1).InsertTable(dataTable);
            worksheet.Columns().AdjustToContents();
        }

        private static DataTable ToDataTable<T>(this IEnumerable<T> entityList) where T : class
        {
            var properties = typeof(T).GetProperties();
            var table = new DataTable();

            // Add columns based on properties
            foreach (var property in properties)
            {
                // Handle Nullable types
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                table.Columns.Add(property.Name, type);
            }

            // Add rows based on entity values
            foreach (var entity in entityList)
            {
                table.Rows.Add(properties.Select(p => p.GetValue(entity, null)).ToArray());
            }

            return table;
        }
    }
}
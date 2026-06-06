
using ClosedXML.Excel;
using LiveChartsCore.SkiaSharpView;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Repository;
using MauiApp1.Modals;
using MauiApp1.Shared;
using System.Text.Json;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    public class BookService
    {
        private Lazy<BookRepository> BookRepository;
        private BookRepository _BookRepository;

        public BookService()
        {
            BookRepository = new Lazy<BookRepository>(() => new BookRepository());
            _BookRepository = BookRepository.Value;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
                return await _BookRepository.GetBooksAsync();
        }

        public async Task<List<Book>> GetBooksNotReadAsync()
        {
            return await _BookRepository.GetBooksNotReadAsync();
        }

        public async Task<Book> GetBookAsync(int id)
        {
            return await _BookRepository.GetBookAsync(id);
        }

        public async Task<int> SaveBookAsync(Book item, string newSeries)
        {
            if (string.IsNullOrWhiteSpace(newSeries) && item.BookSeries > 0)
            {
                return await _BookRepository.SaveBookAsync(item);
            }
            else
            {
                BookSeries bookSeries = new BookSeries {
                    Title = newSeries
                };
                var seriesResult = await _BookRepository.SaveBookSeriesAsync(bookSeries);
                var bookResult = await _BookRepository.SaveBookAsync(item);

                return seriesResult + bookResult;
            }
        }

        public async Task<int> DeleteBookAsync(Book item)
        {
            return await _BookRepository.DeleteBookAsync(item);
        }

        public async Task<List<Publisher>> GetPublishersAsync()
        {
            return await _BookRepository.GetPublishersAsync();
        }

        public async Task<int> SavePublisherAsync(Publisher item)
        {
            return await _BookRepository.SavePublisherAsync(item);
        }

        public async Task<int> DeletePublisherAsync(Publisher item)
        {
            return await _BookRepository.DeletePublisherAsync(item);
        }

        public async Task<List<BookSeries>> GetBookSeriesAsync()
        {
            return await _BookRepository.GetAllBookSeriesAsync();
        }

        public async Task<BookSeries> GetBookSeriesAsync(int id)
        {
            return await _BookRepository.GetBookSeriesAsync(id);
        }

        public async Task<int> SaveBookSeriesAsync(BookSeries item)
        {
            return await _BookRepository.SaveBookSeriesAsync(item);
        }
         public async Task<int> DeleteBookSeriesAsync(BookSeries item)
        {
            return await _BookRepository.DeleteBookSeriesAsync(item);
        }

        public List<TextValuePair<string, int>> GetBookFormats()
        {
            return Enum.GetValues(typeof(BookFormat)).Cast<BookFormat>().Select(g => new TextValuePair<string, int>(g.ToString(), (int)g)).ToList();
        }

         public List<TextValuePair<string, int>> GetBookTypes()
        {
            return Enum.GetValues(typeof(BookType)).Cast<BookType>().Select(g => new TextValuePair<string, int>(g.ToString(), (int)g)).ToList();
        }

        public async Task<bool> ImportBookDataFromExcel(XLWorkbook wb)
        {
            try
            {
                var bookDtos = ReadBooksFromExcel(wb);
                var books = bookDtos.Select(dto => MapDtoToBook(dto)).ToList();
               bool success = await _BookRepository.SaveBooksAsync(books);
                
                var bookSeries = ReadBookSeriesFromExcel(wb);
                success = success && await _BookRepository.SaveBookSeriesAsync(bookSeries);
                
                var publishers = ReadPublishersFromExcel(wb);
                success = success &&  await _BookRepository.SavePublishersAsync(publishers);
               
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing books from Excel: {ex.Message}");
                return false;
            }
        }

        public List<BookExcelDto> ReadBooksFromExcel(XLWorkbook wb)
        {
            var ws = wb.Worksheet(1);
            var headerRow = ws.Row(1);
            var headers = headerRow.CellsUsed()
                .Select((c, i) => new { Name = c.GetString().Trim(), Index = i + 1 })
                .ToDictionary(x => x.Name, x => x.Index);

            var list = new List<BookExcelDto>();
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var dto = new BookExcelDto
                {
                    Id = headers.ContainsKey("Id") ? row.Cell(headers["Id"]).GetString() : null,
                    Title = headers.ContainsKey("Title") ? row.Cell(headers["Title"]).GetString() : null,
                    Author = headers.ContainsKey("Author") ? row.Cell(headers["Author"]).GetString() : null,
                    Artist = headers.ContainsKey("Artist") ? row.Cell(headers["Artist"]).GetString() : null,
                    Publisher = headers.ContainsKey("Publisher") ? row.Cell(headers["Publisher"]).GetString() : null,
                    Genre = headers.ContainsKey("Genre") ? row.Cell(headers["Genre"]).GetString() : null,
                    Format = headers.ContainsKey("Format") ? row.Cell(headers["Format"]).GetString() : null,
                    Volume = headers.ContainsKey("Volume") ? row.Cell(headers["Volume"]).GetString() : null,
                    Read = headers.ContainsKey("Read") ? row.Cell(headers["Read"]).GetString() : null,
                    Language = headers.ContainsKey("Language") ? row.Cell(headers["Language"]).GetString() : null,
                    BookSeries = headers.ContainsKey("BookSeries") ? row.Cell(headers["BookSeries"]).GetString() : null,
                    Type = headers.ContainsKey("Type") ? row.Cell(headers["Type"]).GetString() : null
                };
                list.Add(dto);
            }
            return list;
        }

        public List<Publisher> ReadPublishersFromExcel(XLWorkbook wb)
        {
            var ws = wb.Worksheet(3);
            var headerRow = ws.Row(1);
            var headers = headerRow.CellsUsed()
                .Select((c, i) => new { Name = c.GetString().Trim(), Index = i + 1 })
                .ToDictionary(x => x.Name, x => x.Index);

            var list = new List<Publisher>();
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var dto = new Publisher
                {
                    Id = headers.ContainsKey("Id") ? int.TryParse(row.Cell(headers["Id"]).GetString(), out var id) ? id : 0 : 0,
                    PublisherName = headers.ContainsKey("PublisherName") ? row.Cell(headers["PublisherName"]).GetString() : null,
                };
                list.Add(dto);
            }
            return list;
        }

        public List<BookSeries> ReadBookSeriesFromExcel(XLWorkbook wb)
        {
            var ws = wb.Worksheet(2);
            var headerRow = ws.Row(1);
            var headers = headerRow.CellsUsed()
                .Select((c, i) => new { Name = c.GetString().Trim(), Index = i + 1 })
                .ToDictionary(x => x.Name, x => x.Index);

            var list = new List<BookSeries>();
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var dto = new BookSeries
                {
                    Id = headers.ContainsKey("Id") ? int.TryParse(row.Cell(headers["Id"]).GetString(), out var id) ? id : 0 : 0,
                    Title = headers.ContainsKey("Title") ? row.Cell(headers["Title"]).GetString() : null,
                    Ongoing = headers.ContainsKey("Ongoing") ? row.Cell(headers["Ongoing"]).GetString() == "Y" : false,
                    Collecting = headers.ContainsKey("Collecting") ? row.Cell(headers["Collecting"]).GetString() : null,
                    UpToDateComplete = headers.ContainsKey("UpToDateComplete") ? row.Cell(headers["UpToDateComplete"]).GetString() == "Y" : false,
                    TotalVolumes = headers.ContainsKey("TotalVolumes") ? int.TryParse(row.Cell(headers["TotalVolumes"]).GetString(), out var tv) ? tv : 0 : 0,
                    Demographic = headers.ContainsKey("Demographic") ? int.TryParse(row.Cell(headers["Demographic"]).GetString(), out var d) ? d : 0 : 0,
                    Parent = headers.ContainsKey("Parent") ? row.Cell(headers["Parent"]).GetString() : null
                };
                list.Add(dto);
            }
            return list;
        }

        public Book MapDtoToBook(BookExcelDto dto)
        {

            var genres = dto.Genre.Split(",");
            List<Genre> bookGenre = new List<Genre>();
            foreach (var item in genres)
            {
                bookGenre.Add((Genre)(int.TryParse(item, out var gen) ? gen : 0));
            }

            var book = new Book
            {
                Id = (int.TryParse(dto.Id, out var id) ? id : default),
                Title = dto.Title,
                Author = string.IsNullOrWhiteSpace(dto.Author) ? null : dto.Author,
                Artist = string.IsNullOrWhiteSpace(dto.Artist) ? null : dto.Artist,
                Publisher = int.TryParse(dto.Publisher, out var p) ? p : 0,
                Genre = bookGenre,
                Format = (BookFormat)(int.TryParse(dto.Format, out var f) ? f : default),
                Volume = dto.Volume,
                Read = bool.TryParse(dto.Read, out var r) && r,
                Language = (Language)(int.TryParse(dto.Language, out var l) ? l : 0),
                BookSeries = int.TryParse(dto.BookSeries, out var bs) ? bs : 0,
                Type = (BookType)(int.TryParse(dto.Type, out var t) ? t : default)
            };
            return book;
        }


    }
}

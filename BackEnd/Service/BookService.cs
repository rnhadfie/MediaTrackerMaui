
using ClosedXML.Excel;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Repository;
using MauiApp1.BackEnd.Shared;
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
            List<Book> books = await _BookRepository.GetBooksAsync();
            /*List<BookDT> bookDTs = new List<BookDT>();
            if (books != null && books.Count > 0)
            {
                foreach (var book in books)
                {
                    bookDTs.Add(book.GetBook());
                }
            }*/
            return books;
        }

        public async Task<List<Book>> GetBooksNotReadAsync()
        {
            List<Book> books = await _BookRepository.GetBooksNotReadAsync();
            /*List<BookDT> bookDTs = new List<BookDT>();
            if (books != null && books.Count > 0)
            {
                foreach (var book in books)
                {
                    bookDTs.Add(book.GetBook());
                }
            }*/
            return books;
        }

        public async Task<Book> GetBookAsync(int id)
        {
            Book book = await _BookRepository.GetBookAsync(id);
            return book;
        }

        public async Task<BookItem> GetBookSeriesAsync(int id)
        {
            BookItem bookSeries = await _BookRepository.GetBookSeriesAsync(id);
            return bookSeries;
        }

        public async Task<int> SaveBookAsync(Book item, string newSeries, string newPublisher)
        {
            try
            {
                var result = await _BookRepository.SaveBookAsync(item);
                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<bool> SaveBookSeriesAsync(List<BookItem> series)
        {
            return await _BookRepository.SaveBookSeriesAsync(series);
        }
        public async Task<bool> SaveBookAndSeriesAsync(Book book, List<BookItem> series, string newPublisherName)
        {
            return await _BookRepository.SaveBookAndSeriesAsync(book, series, newPublisherName);
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

        public async Task<List<BookItem>> GetAllBookSeriesAsync()
        {
            return await _BookRepository.GetAllBookSeriesAsync();
        }

        public async Task<int> SaveBookSeriesAsync(BookItem item)
        {
            return await _BookRepository.SaveBookSeriesAsync(item);
        }

        public async Task<int> DeleteBookSeriesAsync(BookItem item)
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

        public async Task<XLWorkbook> ExportBookData(XLWorkbook workBook)
        {
            List<Book> books = await _BookRepository.GetBooksAsync();
            List<Publisher> publishers = await _BookRepository.GetPublishersAsync();
            List<BookItem> bookSeries = await _BookRepository.GetAllBookSeriesAsync();

            var bookSheet = workBook.AddWorksheet("Books");
            bookSheet.Cell(1, 1).InsertData(books, true);

            var pubSheet = workBook.AddWorksheet("Publishers");
            pubSheet.Cell(1, 1).InsertData(publishers, true);

            var seriesSheet = workBook.AddWorksheet("BookSeries");
            seriesSheet.Cell(1, 1).InsertData(publishers, true);

            return workBook;
        }

        /*

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

        public List<BookItem> ReadBookSeriesFromExcel(XLWorkbook wb)
        {
            var ws = wb.Worksheet(2);
            var headerRow = ws.Row(1);
            var headers = headerRow.CellsUsed()
                .Select((c, i) => new { Name = c.GetString().Trim(), Index = i + 1 })
                .ToDictionary(x => x.Name, x => x.Index);

            var list = new List<BookItem>();
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var dto = new BookItem
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
        } */


    }
}

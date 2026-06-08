
using SQLite;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Modals
{
    public class BookDT
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author {  get; set; }
        public string? Artist { get; set; }
        
        public int? Publisher { get; set; }
        public List<Genre> Genre { get; set; }
        public BookFormat Format { get; set; }
        public List<int> Volume { get; set; }
        public bool Read { get; set; }
        public Language Language { get; set; }

        public int BookSeries { get; set;  }
        public BookType? Type { get; set; }

        public Book GetBook()
        {
            return new Book
            {
                Id = this.Id,
                Title = this.Title,
                Author = this.Author,
                Artist = this.Artist,
                Publisher = this.Publisher,
                BookSeries = this.BookSeries,
                Genre = this.Genre.ToString(),
                Format = this.Format,
                Language = this.Language,
                Read = this.Read,
                Volume = this.Volume.ToString(),
                Type = this.Type
            };
        }

    }

    public class Book
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public string? Artist { get; set; }

        public int? Publisher { get; set; }
        public string Genre { get; set; }
        public BookFormat Format { get; set; }
        public string Volume { get; set; }
        public bool Read { get; set; }
        public Language Language { get; set; }

        public int BookSeries { get; set; }
        public BookType? Type { get; set; }

        public BookDT GetBook()
        {
            var genList = this.Genre.Split(",").ToList();
            var bookGenres = new List<Genre>();
            foreach (var item in genList)
            {
                Int32.TryParse(item, out int gen);
                bookGenres.Add((Genre)gen);
            }

            var volumeList = this.Genre.Split(",").ToList();
            var bookVolumes = new List<int>();
            foreach (var item in genList)
            {
                Int32.TryParse(item, out int volume);
                bookVolumes.Add(volume);
            }

            return new BookDT
            {
                Id = this.Id,
                Title = this.Title,
                Author = this.Author,
                Artist = this.Artist,
                Publisher = this.Publisher,
                BookSeries = this.BookSeries,
                Genre = bookGenres,
                Format = this.Format,
                Language = this.Language,
                Read = this.Read,
                Volume = bookVolumes,
                Type = this.Type
            };
        }

    }
}

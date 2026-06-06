
using SQLite;
using System.ComponentModel.DataAnnotations.Schema;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Modals
{
    public class Book
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author {  get; set; }
        public string? Artist { get; set; }
        
        public int? Publisher { get; set; }
        public List<Genre> Genre { get; set; }
        public BookFormat Format { get; set; }
        public string Volume { get; set; }
        public bool Read { get; set; }
        public Language Language { get; set; }

        public int BookSeries { get; set;  }
        public BookType? Type { get; set; }

    }
}

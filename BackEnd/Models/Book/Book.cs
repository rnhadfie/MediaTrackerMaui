
using SQLite;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Models
{
    public class Book
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author {  get; set; }
        public string? Artist { get; set; }
        
        public int? Publisher { get; set; }
        public List<int> Genre { get; set; }

        public BookType Type { get; set; }

        public bool Ongoing { get; set; }
        public bool Collecting { get; set; }
        public bool Completed { get; set; }
        public int Tag { get; set; }

    }

    
}

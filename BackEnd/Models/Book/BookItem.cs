using SQLite;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Models
{
    public class BookItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Series { get; set; }
        public string VolumeNumber { get; set; }
        public string VolumeTitle { get; set; }
        public bool Read { get; set; }
        public bool Owned { get; set; }
        public BookFormat Format { get; set; }
    }
}

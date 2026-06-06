using SQLite;

namespace MauiApp1.BackEnd.Models
{
    public class BookSeries
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Ongoing { get; set; }
        public string Collecting { get; set; }
        public bool UpToDateComplete { get; set; }
        public int TotalVolumes { get; set; }
        public int Demographic { get; set; }
        public string Parent { get; set; }
    }
}

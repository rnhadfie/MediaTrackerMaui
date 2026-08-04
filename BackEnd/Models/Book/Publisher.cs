using SQLite;

namespace MauiApp1.BackEnd.Models
{
    public class Publisher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PublisherName { get; set; }
    }
}

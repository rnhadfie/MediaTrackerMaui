
namespace MauiApp1.BackEnd.Shared
{
    public class Enums
    {
       public enum MediaDataType {
         All = 0,
         Book = 1,
         Video = 2,
         Cd = 3,
         Other = 4,
         Collection = 5,
        }

        public enum DisplayOptions { 
            series = 1,
            book = 2,
            Video = 3,
            Cd = 4,
        }

        public enum CollectionStatus { 
            All = 0,
            NotCompleting = 1,
            OnHold = 2,
            Collecting = 3,
            Completed = 4,
        }

        public enum BookFormat
        {
            Paperback = 1,
            Hardcover = 2,
            EBook = 3
        }

        public enum Genre
        {
            Fiction = 1,
            NonFiction = 2,
            Mystery = 3,
            ScienceFiction = 4,
            Fantasy = 5,
            Biography = 6,
            History = 7,
            Romance = 8,
            Thriller = 9,
            Horror = 10
        }

        public enum  BookType
        {
            Novel = 1,
            Anthology = 2,
            GraphicNovel = 3,
            Manga = 4,
            LightNovel = 5,
        }

        public enum VideoFormat
        {
            DVD = 1,
            Bluray = 2,
            ultraHd = 3
        }

        public enum VideoType { 
            Movie = 1,
            tvShow = 2,
        }

        public enum VideoGroup
        {
            LiveAction = 1,
            Anime = 2,
            WesternAnimation =3,
            Concert = 4,
            Documentary = 5
        }
    }
}


namespace MauiApp1.BackEnd.Shared
{
    public class Enums
    {
        #region Shared 
        public enum Language
        {
            English = 1,
            Spanish = 2,
            French = 3,
            Chinese = 5,
            Japanese = 6,
        }

        public enum Genre
        {
            Action = 1,
            Comedy = 2,
            Drama = 3,
            Mstery = 4,
            Horror = 5,
            ScienceFiction = 6,
            SliceOfLife = 7,
            Fantasy = 8,
            Supernatural = 9,
            Crime = 10,
            YaoiYuri = 11,
            Trillers = 12,
            Romance = 13,
            Psychological = 14,
            IysekaiHealing = 15,
            Historical = 16,
            Music = 17,
        }

        public enum MediaType 
        {
            Book = 1,

        }

        #endregion
        public enum BookFormat
        {
            Paperback = 1,
            Hardcover = 2,
            EBook = 3
        }

   
        public enum BookType
        {
            Novel = 1,
            LightNovel = 2,
            NonFiction = 3,
            Manga = 4,
            GraphicNovel = 5,
            ArtBook = 6,
        }

       
    }
}

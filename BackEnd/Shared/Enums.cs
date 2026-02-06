//using Android.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Shared
{
    public class Enums
    {
       public enum MediaDataType {
         All = 0,
         Book = 1,
         Video = 2,
         Cd = 3,
         Games = 4,
         Series = 5,
        }

        public enum DisplayOptions { 
            series = 1,
            book = 2,
            Video = 3,
            Cd = 4,
        }

        public enum CollectionStatus { 
            NotCompleting = 1,
            OnHold = 2,
            Collecting = 3,
            Completed = 4,
        }
    }
}

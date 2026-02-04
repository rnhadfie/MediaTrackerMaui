
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Service.Modals
{
    public class Series
    {
        [PrimaryKey, AutoIncrement]
        public int SeriesId { get; set; }
        public byte[] TitleImage { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }

        public string Publisher { get; set; }

        public int? TotalVolumes { get; set; }

        public MediaDataType type { get; set; }

        public CollectionStatus CollectionStatus { get; set; }
    }
}

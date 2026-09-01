using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Models.Video
{

    public class Video
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string Name { get; set; }
        public int SeriesId { get; set; }
        public List<int> Genre { get; set; }
        public List<int> Language { get; set; }

        public int Season { get; set; }

        public VideoTag Tag { get; set; }
        public VideoFormat Format { get; set; }

        public bool Watched { get; set; }

        public VideoType Type { get; set; }
    }
}

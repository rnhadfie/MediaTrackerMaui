using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Models.Video
{
    public class VideoItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Series { get; set; }
        public int? DiscNumber { get; set; }
        public string? DiscTitle { get; set; }

        public bool Watched { get; set; }
        public bool Owned { get; set; }
     
        public VideoFormat Format { get; set; }

        public VideoType Type { get; set; }
    } 
}

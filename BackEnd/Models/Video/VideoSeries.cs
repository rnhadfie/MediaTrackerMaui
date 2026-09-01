using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Models.Video
{
    public class VideoSeries
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Ongoing { get; set; }
        public string Collecting { get; set; }
        public bool UpToDateComplete { get; set; }
        public string Parent { get; set; }
    }
}

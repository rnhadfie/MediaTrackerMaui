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
        public int Id { get; set; }
        public string Series { get; set; }
        public List<int> Genre { get; set; }

        public VideoType Type { get; set; }

        public string Collecting { get; set; }
        public bool Ongoing { get; set; }

        public int Tag { get; set; }
    }
}

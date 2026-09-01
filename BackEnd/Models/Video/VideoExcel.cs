using System;
using System.Collections.Generic;
using System.Text;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Models.Video
{
    public class VideoExcel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SeriesId { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }

        public string Season { get; set; }

        public string Tag { get; set; }
        public string Format { get; set; }

        public string Type { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Models
{
    public class BookExcelDto
    {
        public string Id { get; set; }          // Excel values are strings initially
        public string Title { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }       // store CSV or JSON in Excel cell
        public string Format { get; set; }
        public string Volume { get; set; }
        public string Read { get; set; }
        public string Language { get; set; }
        public string BookSeries { get; set; }
        public string Type { get; set; }
    }
}

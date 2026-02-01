using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Service.Modals
{
    public class Book
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string CoverId { get; set; }
        public byte[] Cover { get; set; }
        public string Author {  get; set; }
        public string Series { get; set; }

    }
}

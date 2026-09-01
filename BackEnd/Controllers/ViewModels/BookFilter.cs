using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    public class BookFilter
    {
        public int? Type { get; set; }
        public int? Format { get; set;}

        public int? Genre { get; set; }

        public int? Take { get; set; }

        public int? Series { get; set; }
    }
}

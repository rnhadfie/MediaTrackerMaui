using MauiApp1.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Controllers.ViewModels
{
    public class BookSetupViewModel
    {
        public List<TextValuePair<int>> Series { get; set; }
        public List<TextValuePair<int>> Genre { get; set; }
        public List<TextValuePair<int>> Format { get; set; }
        public List<TextValuePair<int>> Type { get; set; }
    }
}

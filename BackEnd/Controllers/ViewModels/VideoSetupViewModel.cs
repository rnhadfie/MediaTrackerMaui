using MauiApp1.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    public class VideoSetupViewModel
    {
       public List<TextValuePair<int>> Category { get; set;  }
        public List<TextValuePair<int>> Series { get; set; }
        public List<TextValuePair<int>> VideoFormat { get; set; }
        public List<TextValuePair<int>> VideoType { get; set; }

    }
}

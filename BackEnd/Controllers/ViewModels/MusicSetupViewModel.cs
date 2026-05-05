using MauiApp1.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    public class MusicSetupViewModel
    {
        public List<TextValuePair<int>> Collection { get; set; }
        public List<TextValuePair<int>> Genres { get; set; }
        public List<TextValuePair<int>> Langauge { get; set; }
    }
}

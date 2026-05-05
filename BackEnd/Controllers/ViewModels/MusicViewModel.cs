
using MauiApp1.Shared;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    public class MusicViewModel
    {
        public List<TextValuePair<int>> Collection { get; set; }
        public List<TextValuePair<int>> Genres { get; set; }
        public List<TextValuePair<int>> Langauge { get; set; }
    }
}

using MauiApp1.BackEnd.Shared;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    public class SharedService
    {
        public List<TextValuePair<string, int>> GetLanguages()
        {
            return Enum.GetValues(typeof(Language)).Cast<Language>().Select(g => new TextValuePair<string, int>(g.ToString(), (int)g)).ToList();
        }

        public List<TextValuePair<string, int>> GetGenres()
        {
            return Enum.GetValues(typeof(Genre)).Cast<Genre>().Select(g => new TextValuePair<string, int>(g.ToString(), (int)g)).ToList();
        }

        public List<TextValuePair<string, int>> GetMediaTypes()
        {
            return Enum.GetValues(typeof(MediaType)).Cast<MediaType>().Select(g => new TextValuePair<string, int>(g.ToString(), (int)g)).ToList();
        }

    }
}

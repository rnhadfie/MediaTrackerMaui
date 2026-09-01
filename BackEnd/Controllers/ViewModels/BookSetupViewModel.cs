
using System.Collections.ObjectModel;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Shared;

namespace MauiApp1.Controllers.ViewModels
{

    public class BookSetupViewModel
    {
        public ObservableCollection<BookSeries> BookSeries { get; set; }
        public ObservableCollection<TextValuePair<string, int>> Genre { get; set; }
        public ObservableCollection<TextValuePair<string, int>> Format { get; set; }
        public ObservableCollection<TextValuePair<string, int>> Type { get; set; }

        public ObservableCollection<TextValuePair<string, int>> Langauge { get; set; }

        public ObservableCollection<Publisher> Publisher { get; set; }

        public ObservableCollection<BookDT> Books { get; set; }

        public BookSetupViewModel()
        {
            BookSeries = new ObservableCollection<BookSeries>();
            Genre = new ObservableCollection<TextValuePair<string, int>>();
            Format = new ObservableCollection<TextValuePair<string, int>>();
            Type = new ObservableCollection<TextValuePair<string, int>>();
            Langauge = new ObservableCollection<TextValuePair<string, int>>();
            Publisher = new ObservableCollection<Publisher>();
            Books = new ObservableCollection<BookDT>();
        }
    }
}

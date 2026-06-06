using MauiApp1.Modals;
using MauiApp1.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using MauiApp1.BackEnd.Models;

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

        public ObservableCollection<Book> Books { get; set; }

        public BookSetupViewModel()
        {
            BookSeries = new ObservableCollection<BookSeries>();
            Genre = new ObservableCollection<TextValuePair<string, int>>();
            Format = new ObservableCollection<TextValuePair<string, int>>();
            Type = new ObservableCollection<TextValuePair<string, int>>();
            Langauge = new ObservableCollection<TextValuePair<string, int>>();
            Publisher = new ObservableCollection<Publisher>();
            Books = new ObservableCollection<Book>();
        }
    }
}

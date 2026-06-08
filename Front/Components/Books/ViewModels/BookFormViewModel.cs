using MauiApp1.BackEnd.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static MauiApp1.BackEnd.Shared.Enums;
using MauiApp1.BackEnd.Shared;
using System.ComponentModel;

namespace MauiApp1.Front.Components.Books.ViewModels
{
    public class BookFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Observable<int> Id { get; set; } = new Observable<int>(0);
        public Observable<string> Title { get; set; } = new Observable<string>(string.Empty);
        public Observable<string?> Author { get; set; } = new Observable<string?>(null);
        public Observable<string?> Artist { get; set; } = new Observable<string?>(null);

        public Observable<int?> Publisher { get; set; } = new Observable<int?>(null);
        public ObservableCollection<int> Genre { get; set; } = new ObservableCollection<int>();
        public Observable<int> Format { get; set; } = new Observable<int>(0);
        public Observable<string?> Volume { get; set; } = new Observable<string?>(null);
        public Observable<bool> Read { get; set; } = new Observable<bool>(false);
        public Observable<Language> Language { get; set; } = new Observable<Language>(default!);

        // Selected items from multi-picker (stores selected option objects)
        public List<TextValuePair<string, int>> SelectedGenres { get; set; } = new List<TextValuePair<string, int>>();

        public Observable<int> BookSeries { get; set; } = new Observable<int>(0);

        public Observable<bool> IsSeriesSelected { get; set; } = new Observable<bool>(false);
        public Observable<BookType?> Type { get; set; } = new Observable<BookType?>(null);

        public Observable<string?> NewBookSeriesName { get; set; } = new Observable<string?>(null);

        public Observable<string?> NewPublisher { get; set; } = new Observable<string?>(null);
        public List<TextValuePair<string, int>> BookSeriesList { get; set; } = new List<TextValuePair<string, int>>();
        public List<TextValuePair<string, int>> Publishers { get; set; } = new List<TextValuePair<string, int>>();

        public List<TextValuePair<string, int>> GenreOptions { get; set; } = new List<TextValuePair<string, int>>();

        public bool EnableNewSeries => BookSeries.Value <= 0;

        public bool EnablePublisher => Publisher.Value == null || Publisher.Value <= 0;
    }
}

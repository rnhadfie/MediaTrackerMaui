using MauiApp1.BackEnd.Shared;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Books.ViewModels
{
    public class BookFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public BookFormViewModel()
        {
            // Subscribe to observables we depend on so computed    properties update UI
            Id.PropertyChanged += (_, __) => Notify(nameof(Id));
            Title.PropertyChanged += (_, __) => Notify(nameof(Title));
            Author.PropertyChanged += (_, __) => Notify(nameof(Author));
            Artist.PropertyChanged += (_, __) => Notify(nameof(Artist));
            IsEdit.PropertyChanged += (_, __) => Notify(nameof(IsEdit));
            Format.PropertyChanged += (_, __) => Notify(nameof(Format));
            Language.PropertyChanged += (_, __) => Notify(nameof(Language));
            Genre.CollectionChanged += (_, __) => Notify(nameof(Genre));
            IsEdit.PropertyChanged += (_, __) => Notify(nameof(IsEdit));
            GenreOptions.CollectionChanged += OnSelectedItemsChanged;

            BookSeries.PropertyChanged += (_, __) =>
            {
                Notify(nameof(EnableNewSeries));
                // sync selected object when the id changes
                SyncSelectedBookSeriesFromBookSeries();
            };

            Publisher.PropertyChanged += (_, __) =>
            {
                Notify(nameof(EnablePublisher));
                SyncSelectedPublisherFromPublisher();
            };

            // Keep SelectedBookSeries/SelectedPublisher in sync when UI selects items
            SelectedBookSeries.PropertyChanged += (_, __) =>
            {
                // when selected object changes, update the id value
                var sel = SelectedBookSeries.Value;
                BookSeries.Value = sel?.Value ?? 0;
                Notify(nameof(SelectedBookSeries));
            };
            SelectedPublisher.PropertyChanged += (_, __) =>
            {
                var sel = SelectedPublisher.Value;
                Publisher.Value = sel?.Value;
                Notify(nameof(SelectedPublisher));
            };
        }

        private void SyncSelectedPublisherFromPublisher()
        {
            try
            {
                if (Publishers == null) return;
                var match = Publishers.Find(p => p.Value == Publisher.Value.GetValueOrDefault());
                if (match != null)
                {
                    if (!EqualityComparer<TextValuePair<string,int>?>.Default.Equals(SelectedPublisher.Value, match))
                    {
                        SelectedPublisher.Value = match;
                    }
                }
                else
                {
                    SelectedPublisher.Value = null;
                }
            }
            catch
            {
                // best-effort sync
            }
        }

        private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public Observable<int> Id { get; set; } = new Observable<int>(0);
        public Observable<string> Title { get; set; } = new Observable<string>(string.Empty);
        public Observable<string?> Author { get; set; } = new Observable<string?>(null);
        public Observable<string?> Artist { get; set; } = new Observable<string?>(null);

        public Observable<int?> Publisher { get; set; } = new Observable<int?>(null);
        public ObservableCollection<int> Genre { get; set; } = new ObservableCollection<int>();
        public Observable<int> Format { get; set; } = new Observable<int>(0);
        public Observable<string?> Volume { get; set; } = new Observable<string?>(null);
        public Observable<bool> Read { get; set; } = new Observable<bool>(false);
        public Observable<bool> IsEdit { get; set; } = new Observable<bool>(true);
        public Observable<Language> Language { get; set; } = new Observable<Language>(default!);

        // Selected items from multi-picker (stores selected option objects)
        public List<TextValuePair<string, int>> SelectedGenres { get; set; } = new List<TextValuePair<string, int>>();



        // When a picker selects a TextValuePair, store it here so SelectedItem binding has correct type
        public Observable<TextValuePair<string,int>?> SelectedBookSeries { get; set; } = new Observable<TextValuePair<string,int>?>(null);
        public Observable<TextValuePair<string,int>?> SelectedPublisher { get; set; } = new Observable<TextValuePair<string,int>?>(null);

        public Observable<int> BookSeries { get; set; } = new Observable<int>(0);

        public Observable<bool> IsSeriesSelected { get; set; } = new Observable<bool>(false);
        public Observable<BookType?> Type { get; set; } = new Observable<BookType?>(null);

        public Observable<string?> NewBookSeriesName { get; set; } = new Observable<string?>(null);

        public Observable<string?> NewPublisher { get; set; } = new Observable<string?>(null);
        public List<TextValuePair<string, int>> BookSeriesList { get; set; } = new List<TextValuePair<string, int>>();
        public List<TextValuePair<string, int>> Publishers { get; set; } = new List<TextValuePair<string, int>>();

        public ObservableCollection<TextValuePair<string, int>> GenreOptions { get; set; } = new ObservableCollection<TextValuePair<string, int>>();

        public bool EnableNewSeries => BookSeries.Value <= 0;

        public bool EnablePublisher => Publisher.Value == null || Publisher.Value <= 0;

        private void SyncSelectedBookSeriesFromBookSeries()
        {
            try
            {
                if (BookSeriesList == null) return;
                var match = BookSeriesList.Find(p => p.Value == BookSeries.Value);
                if (match != null)
                {
                    // avoid reassigning same object reference
                    if (!EqualityComparer<TextValuePair<string,int>?>.Default.Equals(SelectedBookSeries.Value, match))
                    {
                        SelectedBookSeries.Value = match;
                    }
                }
                else
                {
                    SelectedBookSeries.Value = null;
                }
            }
            catch
            {
                // swallow - best effort sync
            }
        }
    }
}


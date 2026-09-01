using MauiApp1.BackEnd.Shared;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using static MauiApp1.BackEnd.Shared.Enums;
using MauiApp1.BackEnd.Models;
using MauiApp1.Controllers;
using System.Threading.Tasks;
using System.Linq;

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
            Genre.CollectionChanged += (_, __) => Notify(nameof(Genre));
            IsEdit.PropertyChanged += (_, __) => Notify(nameof(IsEdit));
            GenreOptions.CollectionChanged += OnSelectedItemsChanged;

            Publisher.PropertyChanged += (_, __) =>
            {
                Notify(nameof(EnablePublisher));
                SyncSelectedPublisherFromPublisher();
            };

            // Keep SelectedBookSeries/SelectedPublisher in sync when UI selects items
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
                var match = Publishers.FirstOrDefault(p => p.Value == Publisher.Value.GetValueOrDefault());
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

        // Load a book by id and populate viewmodel fields
        public async Task LoadAsync(int id)
        {
            if (id <= 0) return;

            var controller = new BookController();
            var book = await controller.GetBook(id);
            if (book == null) return;

            // Set the Value on existing Observable<T> instances so bindings remain intact
            Id.Value = book.Id;
            Title.Value = book.Title;
            Author.Value = book.Author;
            Artist.Value = book.Artist;
            Publisher.Value = book.Publisher;

            // Map genres (book.Genre -> selected ids)
            Genre = new ObservableCollection<int>(book.Genre?.Select(g => (int)g) ?? new List<int>());

            // Map selected genre objects if GenreOptions already loaded
            try
            {
                SelectedGenres = GenreOptions?.Where(opt => Genre.Contains(opt.Value)).ToList() ?? new List<TextValuePair<string,int>>();
            }
            catch
            {
                SelectedGenres = new List<TextValuePair<string,int>>();
            }

            Type.Value = book.Type;
        }

        private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public Observable<int> Id { get; set; } = new Observable<int>(0);
        public Observable<string> Title { get; set; } = new Observable<string>(string.Empty);
        public Observable<string?> Author { get; set; } = new Observable<string?>(null);
        public Observable<string?> Artist { get; set; } = new Observable<string?>(null);

        public Observable<int?> Publisher { get; set; } = new Observable<int?>(null);

        public Observable<string?> NewPublisher { get; set; } = new Observable<string?>(null);

        public ObservableCollection<int> Genre { get; set; } = new ObservableCollection<int>();

        public Observable<BookType> Type { get; set; } = new Observable<BookType>(BookType.Novel);

        public Observable<bool> IsEdit { get; set; } = new Observable<bool>(true);

        public Observable<bool> Ongoing { get; set; } = new Observable<bool>(true);

        public Observable<bool> Collecting { get; set; } = new Observable<bool>(false);
        public Observable<bool> Completed { get; set; } = new Observable<bool>(false);
        public Observable<int> Tag { get; set; } = new Observable<int>(-1);

        public Observable<string?> NewTag { get; set; } = new Observable<string?>(null);

        // Selected items from multi-picker (stores selected option objects)
        public List<TextValuePair<string, int>> SelectedGenres { get; set; } = new List<TextValuePair<string, int>>();



        // When a picker selects a TextValuePair, store it here so SelectedItem binding has correct type
        public Observable<TextValuePair<string,int>?> SelectedBookSeries { get; set; } = new Observable<TextValuePair<string,int>?>(null);
        public Observable<TextValuePair<string,int>?> SelectedPublisher { get; set; } = new Observable<TextValuePair<string,int>?>(null);

        public Observable<TextValuePair<string, int>?> SelectedTag{ get; set; } = new Observable<TextValuePair<string, int>?>(null);

        public Observable<int> BookSeries { get; set; } = new Observable<int>(0);


        public Observable<string?> NewBookSeriesName { get; set; } = new Observable<string?>(null);

        
        public ObservableCollection<TextValuePair<string, int>> Publishers { get; set; } = new ObservableCollection<TextValuePair<string, int>>();

        public ObservableCollection<TextValuePair<string, int>> Tags { get; set; } = new ObservableCollection<TextValuePair<string, int>>();

        public ObservableCollection<TextValuePair<string, int>> GenreOptions { get; set; } = new ObservableCollection<TextValuePair<string, int>>();

        // Load setup data (publishers, genres, tags) into the view model
        public async Task LoadSetupAsync()
        {
            var controller = new BookController();
            var setup = await controller.GetBookSetup();

            if (setup?.Genre != null)
            {
                GenreOptions = setup.Genre;
            }

            // Map publishers from setup Publisher collection if present
            try
            {
                var pubs = await controller.GetPublishersAsync();
                Publishers = new ObservableCollection<TextValuePair<string, int>>(pubs.Select(p => new TextValuePair<string, int>(p.PublisherName ?? string.Empty, p.Id)).ToList());
            }
            catch
            {
                // ignore publisher load errors
            }
        }

        // Book items for the current book (volumes/entries)
        public ObservableCollection<BookItem> BookItems { get; set; } = new ObservableCollection<BookItem>();

        public void AddBookItem(BookItem item)
        {
            if (item == null) return;
            BookItems.Add(item);
            SortBookItems();
            Notify(nameof(BookItems));
        }

        public void RemoveBookItem(BookItem item)
        {
            if (item == null) return;
            BookItems.Remove(item);
            Notify(nameof(BookItems));
        }

        public void SortBookItems()
        {
            try
            {
                var ordered = BookItems.OrderBy(b =>
                {
                    if (int.TryParse(b.VolumeNumber, out var n)) return (object)n;
                    return (object)(b.VolumeNumber ?? b.VolumeTitle ?? string.Empty);
                }).ToList();

                BookItems.Clear();
                foreach (var it in ordered) BookItems.Add(it);
            }
            catch
            {
                // ignore ordering errors
            }
        }


        public bool EnablePublisher => Publisher.Value == null || Publisher.Value <= 0;

        public bool EnableTag => Tag.Value == -1;
    }
}


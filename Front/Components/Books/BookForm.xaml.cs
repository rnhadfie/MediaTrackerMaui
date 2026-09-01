using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Controllers;
using MauiApp1.Front.Components.Books.ViewModels;
using System.Collections.ObjectModel;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Books
{
    [QueryProperty(nameof(id), "id")] // For objects
    [QueryProperty(nameof(edit), "edit")]
    public partial class BookForm : ContentPage, IQueryAttributable
    {
        private int _id;
        private bool _edit;
        public int id;
        public bool edit = true;
        private readonly BookController _controller;

        public BookForm()
        {
            InitializeComponent();
            _id = id;
            _edit = edit; 
            _controller = new BookController();

        }

        private async 
        Task
        LoadBook(int id)
        {
            // kept for synchronous callers - call the async variant where possible
            var book = await _controller.GetBook(id);
            
            if (book == null) return;

            var vm = (BindingContext as BookFormViewModel) ?? new BookFormViewModel();

            // Set the Value on existing Observable<T> instances so bindings remain intact
            vm.Id.Value = book.Id;
            vm.Title.Value = book.Title;
            vm.Author.Value = book.Author;
            vm.Artist.Value = book.Artist;
            vm.Publisher.Value = book.Publisher;
            vm.Volume.Value = string.Join(',', book.Volume ?? new List<int>());
            vm.Genre = new ObservableCollection<int>(book.Genre?.Select(g => (int)g) ?? new List<int>());
            vm.Format.Value = (int)book.Format;
            vm.Read.Value = book.Read;
            vm.Language.Value = book.Language;
            vm.BookSeries.Value = book.BookSeries;
            vm.Type.Value = book.Type;

            vm.IsEdit = _edit;


            // map genres: book.Genre is List<Genre> (enum) -> map to TextValuePair list if available in setup
            // Leave SelectedGenres empty for now; Set via async setup loader when available


            BindingContext = vm;

            BookGenrePicker.SelectedValues = vm.Genre;
            //BookLanguageGroup.SelectedItem = vm.Language;
            BookFormatGroup.SelectedValue = vm.Format;
        }

        private void DisableFields(bool isEnabled)
        {
            BookArtist.IsEnabled = isEnabled;
            BookTitle.IsEnabled = isEnabled;
            BookAuthor.IsEnabled = isEnabled;
            BookPublisherPicker.IsEnabled = isEnabled;
            BookFormatGroup.IsEnabled = isEnabled;
            BookGenrePicker.IsEnabled = isEnabled;
            NewPublisher.IsEnabled = isEnabled;
            NewBookSeries.IsEnabled = isEnabled;
            BookSeriesPicker.IsEnabled = isEnabled;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // load setup and book data asynchronously and update ViewModel
            var setup = await _controller.GetBookSetup();
            await LoadBook(_id);

            var vm = (BindingContext as BookFormViewModel) ?? new BookFormViewModel();

            // populate lists
            vm.BookSeriesList = setup.BookSeries?.Select(s => new TextValuePair<string,int>(s.Title, s.Id)).ToList() ?? new List<TextValuePair<string,int>>();
            vm.Publishers = setup.Publisher?.Select(p => new TextValuePair<string,int>(p.PublisherName ?? string.Empty, p.Id)).ToList() ?? new List<TextValuePair<string,int>>();
            vm.GenreOptions = setup.Genre != null
                ? setup.Genre
                : new ObservableCollection<TextValuePair<string, int>>();

            BookSeriesPicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.BookSeriesList);
            BookGenrePicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.GenreOptions);


            BookPublisherPicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.Publishers);

            List<TextValuePair<string, int>> formats = new List<TextValuePair<string, int>>();
            foreach (var item in setup.Format)
            {
                UraniumUI.Material.Controls.RadioButton radioButton = new UraniumUI.Material.Controls.RadioButton();
                formats.Add(new TextValuePair<string, int>(item.Text, item.Value));
            }
            BookFormatGroup.ItemsSource = formats;

            foreach (var item in setup.Langauge)
            {
                UraniumUI.Material.Controls.RadioButton radioButton = new UraniumUI.Material.Controls.RadioButton();
                radioButton.Text = item.Text;
                radioButton.Value = item.Value;
                //BookLanguageGroup.Children.Add(radioButton);
            }

            // If an id was provided, load the book and map genre selections
            if (_id > 0)
            {
                var book = await _controller.GetBookAsync(_id);
                if (book != null)
                {
                    vm.Id.Value = book.Id;
                    vm.Title.Value = book.Title;
                    vm.Author.Value = book.Author;
                    vm.Artist.Value = book.Artist;
                    vm.Publisher.Value = book.Publisher;
                    vm.Volume.Value = string.Join(',', book.Volume ?? new List<int>());
                    vm.Format.Value = (int)book.Format;
                    vm.Read.Value = book.Read;
                    vm.Language.Value = book.Language;
                    vm.BookSeries.Value = book.BookSeries;
                    vm.Type.Value = book.Type;
                }
            }

            BindingContext = vm;
        }

        // IQueryAttributable implementation - called when navigating with Shell query parameters
        // Example navigation: Shell.Current.GoToAsync($"bookform?id={id}");
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query == null)
                return;

            if (query.TryGetValue("edit", out var editObj) || query.TryGetValue("Edit", out editObj))
            {
                if (editObj is string editStr && bool.TryParse(editStr, out var editBool))
                {
                    _edit = editBool;
                }
                else if (editObj is bool editBoolVal)
                {
                    _edit = editBoolVal;
                }
                DisableFields(_edit);
            }

            if (query.TryGetValue("id", out var idObj) || query.TryGetValue("Id", out idObj))
            {
                int parsed = 0;
                if (idObj is string s && int.TryParse(s, out parsed))
                {
                    _id = parsed;
                }
                else if (idObj is int i)
                {
                    _id = i;
                }

                if (_id > 0)
                {
                    LoadBook(_id);
                }
            }

            
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("bookhome");
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            // Handle save
            var vm = BindingContext as BookFormViewModel;
            if (vm == null)
            {
                await DisplayAlert("Error", "Unable to read form data.", "OK");
                return;
            }

            try
            {
                var book = new BookDT
                {
                    Id = vm.Id.Value,
                    Title = vm.Title.Value,
                    Author = vm.Author.Value,
                    Artist = vm.Artist.Value,
                    Publisher = vm.Publisher.Value,
                    Volume = vm.Volume.Value != null ? vm.Volume.Value.Split(',').Select(s => int.TryParse(s, out var v) ? v : 0).ToList() : new List<int>(),
                    Format = (BookFormat)vm.Format.Value,
                    Read = vm.Read.Value,
                    Language = vm.Language.Value,
                    BookSeries = vm.BookSeries.Value,
                    Type = vm.Type.Value
                };

                

                // Map selected genres (if any) to enum list
                if (vm.SelectedGenres != null && vm.SelectedGenres.Count > 0)
                {
                    book.Genre = vm.SelectedGenres.Select(g => g.Value).ToList();
                }
                else
                {
                    book.Genre = new List<int>();
                }

                var result = await _controller.SaveBookAsync(book, vm.NewBookSeriesName, vm.NewPublisher);
                if (result)
                {
                    await DisplayAlert("Saved", "Book saved successfully.", "OK");
                    await Shell.Current.GoToAsync("bookhome");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save book.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Save failed: {ex.Message}", "OK");
            }
        }
    }
}

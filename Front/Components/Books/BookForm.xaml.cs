
using MauiApp1.BackEnd.Models;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Front.Components.Books.ViewModels;
using MauiApp1.Modals;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UraniumUI.Material;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Books
{
    public partial class BookForm : ContentPage, IQueryAttributable
    {
        private int _id;
        private readonly BookController _controller;

        public BookForm() : this(0)
        {

        }

        public BookForm(int id)
        {
            InitializeComponent();
            _id = id;
            _controller = new BookController();

           var setup=  _controller.GetBookSetup();
            if (_id > 0)
            {
                LoadBook(_id);
            }
            
        }

        private void LoadBook(int id)
        {
            // kept for synchronous callers - call the async variant where possible
            var book = _controller.GetBook(id);
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

            // map genres: book.Genre is List<Genre> (enum) -> map to TextValuePair list if available in setup
            // Leave SelectedGenres empty for now; Set via async setup loader when available

            BindingContext = vm;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // load setup and book data asynchronously and update ViewModel
            var setup = await _controller.GetBookSetup();

            var vm = (BindingContext as BookFormViewModel) ?? new BookFormViewModel();

            // populate lists
            vm.BookSeriesList = setup.BookSeries?.Select(s => new TextValuePair<string,int>(s.Title, s.Id)).ToList() ?? new List<TextValuePair<string,int>>();
            vm.Publishers = setup.Publisher?.Select(p => new TextValuePair<string,int>(p.PublisherName ?? string.Empty, p.Id)).ToList() ?? new List<TextValuePair<string,int>>();
            vm.GenreOptions = setup.Genre?.Select(g => new TextValuePair<string,int>(g.Text, g.Value)).ToList() ?? new List<TextValuePair<string,int>>();

            BookSeriesPicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.BookSeriesList);
            //BookGenrePicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.GenreOptions);
            BookPublisherPicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.Publishers);

            foreach (var item in setup.Format)
            {
                UraniumUI.Material.Controls.RadioButton radioButton = new UraniumUI.Material.Controls.RadioButton();
                radioButton.Text = item.Text;
                radioButton.Value = item.Value;
                BookFormatGroup.Children.Add(radioButton);
            }

            foreach (var item in setup.Langauge)
            {
                UraniumUI.Material.Controls.RadioButton radioButton = new UraniumUI.Material.Controls.RadioButton();
                radioButton.Text = item.Text;
                radioButton.Value = item.Value;
                BookLanguageGroup.Children.Add(radioButton);
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
            await Shell.Current.GoToAsync("BookHome");
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
                    book.Genre = vm.SelectedGenres.Select(g => (Genre)g.Value).ToList();
                }
                else
                {
                    book.Genre = new List<Genre>();
                }

                var result = await _controller.SaveBookAsync(book, vm.NewBookSeriesName);
                if (result >= 0)
                {
                    await DisplayAlert("Saved", "Book saved successfully.", "OK");
                    await Shell.Current.GoToAsync("BookHome");
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

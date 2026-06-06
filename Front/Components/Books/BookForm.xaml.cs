
using MauiApp1.Controllers;
using MauiApp1.Front.Components.Books.ViewModels;
using MauiApp1.BackEnd.Models;
using MauiApp1.Controllers.ViewModels;
using System.Collections.Generic;
using static MauiApp1.BackEnd.Shared.Enums;
using System.Linq;
using System.Threading.Tasks;
using MauiApp1.Modals;
using UraniumUI.Material;

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

            vm.Id = book.Id;
            vm.Title = book.Title;
            vm.Author = book.Author;
            vm.Artist = book.Artist;
            vm.Publisher = book.Publisher;
            vm.Volume = book.Volume;
            vm.Genre = book.Genre?.Select(g => (int)g).ToArray() ?? new int[0];
            vm.Format = (int)book.Format;
            vm.Read = book.Read;
            vm.Language = book.Language;
            vm.BookSeries = book.BookSeries;
            vm.Type = book.Type;

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
                    vm.Id = book.Id;
                    vm.Title = book.Title;
                    vm.Author = book.Author;
                    vm.Artist = book.Artist;
                    vm.Publisher = book.Publisher;
                    vm.Volume = book.Volume;
                    vm.Format = (int)book.Format;
                    vm.Read = book.Read;
                    vm.Language = book.Language;
                    vm.BookSeries = book.BookSeries;
                    vm.Type = book.Type;
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
                var book = new Book
                {
                    Id = vm.Id,
                    Title = vm.Title,
                    Author = vm.Author,
                    Artist = vm.Artist,
                    Publisher = vm.Publisher,
                    Volume = vm.Volume ?? string.Empty,
                    Format = (BookFormat)vm.Format,
                    Read = vm.Read,
                    Language = vm.Language,
                    BookSeries = vm.BookSeries,
                    Type = vm.Type
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

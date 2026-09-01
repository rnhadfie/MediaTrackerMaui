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
    public partial class BookSeriesForm : ContentPage, IQueryAttributable
    {
        private int _id;
        private bool _edit;
        public int id;
        public bool edit = true;
        private readonly BookController _controller;

        public BookSeriesForm()
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
            var bookSeries = await _controller.GetBookSeriesAsync(id);
            
            if (bookSeries == null) return;

            var vm = (BindingContext as BookSeriesFormViewModel) ?? new BookSeriesFormViewModel();

            // Ensure the viewmodel's edit mode matches the parsed query value
            vm.IsEdit.Value = _edit;

            // Set the Value on existing Observable<T> instances so bindings remain intact
            vm.Id.Value = bookSeries.Id;
            //vm.Title.Value = bookSeries.Title;
            //vm.Author.Value = bookSeries.Author;
            //vm.Artist.Value = bookSeries.Artist;
   
            BindingContext = vm;
        }

        private void DisableFields(bool isEnabled)
        {
            BookSeriesArtist.IsEnabled = isEnabled;
            BookSeriesTitle.IsEnabled = isEnabled;
            BookSeriesAuthor.IsEnabled = isEnabled;
            BookSeriesCollecting.IsEdit = isEnabled;
            BookSeriesParent.IsEnabled = isEnabled;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // load setup and book data asynchronously and update ViewModel
            var setup = await _controller.GetBookSetup();
            await LoadBook(_id);

            var vm = (BindingContext as BookSeriesFormViewModel) ?? new BookSeriesFormViewModel();



            List<TextValuePair<string, int>> formats = new List<TextValuePair<string, int>>();
            foreach (var item in setup.Format)
            {
                UraniumUI.Material.Controls.RadioButton radioButton = new UraniumUI.Material.Controls.RadioButton();
                formats.Add(new TextValuePair<string, int>(item.Text, item.Value));
            }

            // If an id was provided, load the book and map genre selections
            if (_id > 0)
            {
                var bookSeries = await _controller.GetBookSeriesAsync(_id);
                if (bookSeries != null)
                {
                    vm.Id.Value = bookSeries.Id;
                    //vm.Title.Value = bookSeries.Title;
                    //vm.Author.Value = bookSeries.Author;
                    //vm.Artist.Value = bookSeries.Artist;
                    //vm.Collecting.Value = bookSeries.Collecting;
                    //vm.Ongoing.Value = bookSeries.Ongoing;
                    //vm.Parent.Value = bookSeries.Parent;
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
            // Navigate back to the Books home (series are embedded there)
            await Shell.Current.GoToAsync("books");
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            // Handle save
            var vm = BindingContext as BookSeriesFormViewModel;
            if (vm == null)
            {
                await DisplayAlert("Error", "Unable to read form data.", "OK");
                return;
            }

            try
            {
                var bookSeries = new BookItem
                {
                    Id = vm.Id.Value,
                   // Title = vm.Title.Value,
                   // Author = vm.Author.Value,
                    // Artist = vm.Artist.Value,
                  //  Collecting = vm.Collecting.Value,
                  //  Ongoing = vm.Ongoing.Value,
                  //  Parent = vm.Parent.Value,
                  //  UpToDateComplete = vm.UpToDateComplete.Value,
                };

                

              
                var result = await _controller.SaveBookSeriesAsync(bookSeries);
                if (result > 0)
                {
                    await DisplayAlert("Saved", "Book series saved successfully.", "OK");
                    await Shell.Current.GoToAsync("books");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save book series.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Save failed: {ex.Message}", "OK");
            }
        }
    }
}

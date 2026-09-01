using System.Linq;
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

        // Book loading moved to the ViewModel: call await vm.LoadAsync(id) instead

        private void DisableFields(bool isEnabled)
        {
            BookArtist.IsEnabled = isEnabled;
            BookTitle.IsEnabled = isEnabled;
            BookAuthor.IsEnabled = isEnabled;
            BookPublisherPicker.IsEnabled = isEnabled;
            BookGenrePicker.IsEnabled = isEnabled;
            NewPublisher.IsEnabled = isEnabled;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var vm = (BindingContext as BookFormViewModel) ?? new BookFormViewModel();

            // let the VM load setup data (genres, publishers, etc.)
            await vm.LoadSetupAsync();

            // load the book data and leave selections in the VM via ViewModel
            await vm.LoadAsync(_id);

            // Populate BookItems for this book from the setup (BookSeries in setup are actually BookItem entries)
            try
            {
                var setup = await _controller.GetBookSetup();
                var items = setup.BookSeries?.Where(i => i.Series == vm.Id.Value).OrderBy(i =>
                {
                    if (int.TryParse(i.VolumeNumber, out var n)) return (object)n;
                    return (object)(i.VolumeNumber ?? i.VolumeTitle ?? string.Empty);
                }).ToList() ?? new List<BookItem>();

                vm.BookItems = new ObservableCollection<BookItem>(items);
            }
            catch
            {
                vm.BookItems = new ObservableCollection<BookItem>();
            }

            BookGenrePicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.GenreOptions);

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
                    // kick off VM load (fire-and-forget to mirror previous behavior)
                    var vm = (BindingContext as BookFormViewModel) ?? new BookFormViewModel();
                    _ = vm.LoadAsync(_id);
                    BindingContext = vm;
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
                var book = new Book
                {
                    Id = vm.Id.Value,
                    Title = vm.Title.Value,
                    Author = vm.Author.Value,
                    Artist = vm.Artist.Value,
                    Publisher = vm.Publisher.Value,
                    //Type = vm.Type.Value!
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

                // Save book and its items in bulk
                var result = await _controller.SaveBookAsync(book, vm.BookItems?.ToList(), vm.NewBookSeriesName, vm.NewPublisher);
                if (result)
                {
                    await DisplayAlert("Saved", "Book and items saved successfully.", "OK");
                    await Shell.Current.GoToAsync("bookhome");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save book and/or items.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Save failed: {ex.Message}", "OK");
            }
        }

        private async void OnAddItemClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as BookFormViewModel;
            if (vm == null) return;

            // create a new blank BookItem and persist it
            var newItem = new BookItem
            {
                Series = vm.Id.Value,
                VolumeNumber = string.Empty,
                VolumeTitle = "",
                Read = false,
                Owned = false,
                Format = 0
            };

            try
            {
                var id = await _controller.SaveBookSeriesAsync(newItem);
                if (id > 0)
                {
                    newItem.Id = id;
                }
            }
            catch
            {
                // ignore persistence errors for now
            }

            vm.AddBookItem(newItem);
        }

        private async void OnRemoveItemClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as BookFormViewModel;
            if (vm == null) return;
            if (sender is Button btn && btn.CommandParameter is BookItem item)
            {
                var confirm = await DisplayAlert("Confirm", "Remove this item?", "Yes", "No");
                if (!confirm) return;

                try
                {
                    if (item.Id != 0)
                    {
                        await _controller.DeleteBookSeriesAsync(item);
                    }
                }
                catch
                {
                    // ignore deletion errors
                }

                vm.RemoveBookItem(item);
            }
        }

        private async void OnEditItemClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as BookFormViewModel;
            if (vm == null) return;
            if (sender is Button btn && btn.CommandParameter is BookItem item)
            {
                await EditBookItemAsync(item);
                vm.SortBookItems();
            }
        }

        private async Task EditBookItemAsync(BookItem item)
        {
            if (item == null) return;
            try
            {
                // Prompt for Volume Number (allow empty)
                var volNum = await DisplayPromptAsync("Edit Item", "Volume number (leave empty if none)", initialValue: item.VolumeNumber ?? string.Empty);
                if (volNum != null)
                {
                    item.VolumeNumber = volNum;
                }

                // Prompt for Volume Title
                var volTitle = await DisplayPromptAsync("Edit Item", "Volume title", initialValue: item.VolumeTitle ?? string.Empty);
                if (volTitle != null)
                {
                    item.VolumeTitle = volTitle;
                }

                // persist changes if possible
                try
                {
                    var id = await _controller.SaveBookSeriesAsync(item);
                    if (id > 0)
                    {
                        item.Id = id;
                    }
                }
                catch
                {
                    // ignore persistence errors for now
                }
                // update vm collection ordering
                var vm = BindingContext as BookFormViewModel;
                vm?.SortBookItems();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Edit failed: {ex.Message}", "OK");
            }
        }

       
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Controllers.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using UraniumUI.Dialogs;
using UraniumUI.Extensions;
using System.Linq;

namespace MauiApp1.Front.Components.Books.ViewModels
{
    public partial class BookHomeViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        public BookHomeViewModel(IDialogService dialogService) { 
            ViewModel = new BookSystemViewModel();
            ViewModel.PropertyChanged += OnSystemViewModelPropertyChanged;

            _dialogService = dialogService;

        }

        public BookHomeViewModel()
        {
            ViewModel = new BookSystemViewModel();
            ViewModel.PropertyChanged += OnSystemViewModelPropertyChanged;

        }


        // ICommand for opening add form
        private ICommand addBookCommand;
        public ICommand AddBookCommand => addBookCommand ??= new Command(async () => await OpenAddForm());

        private async Task OpenAddForm()
        {
            // navigate to BookForm. Ensure Shell route is registered or use absolute path
            await Shell.Current.GoToAsync("/BookForm");
            /*
            var model = new BookFormViewModel();

            var result = await _dialogService.DisplayFormViewAsync(
            title: "Add Book",
            viewModel: model,
            submit: "Save",
            cancel: "Cancel"
        );*/
        }

        private void OnSystemViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Setup) && ViewModel.Setup != null)
            {
                BookSetup = ViewModel.Setup;
                var types = ViewModel.Setup.Type;
                var all = new TextValuePair<string, int>("All", 0);
                types.Insert(0, all);
                TypeOptions = new ObservableCollection<TextValuePair<string, int>>(ViewModel.Setup.Type);
                if (TypeOptions.Count > 0)
                {
                    SelectedType = TypeOptions[0];
                }

                var source = ViewModel?.Setup?.BookSeries ?? Enumerable.Empty<BookItem>();
                if (SelectedType == null || SelectedType.Value == 0)
                {
                    ListOfBookSeries = new ObservableCollection<BookItem>(source);
                }

                // Initialize the book list based on the selected type
                UpdateListOfBooks();

            }
        }

        // Called by the source-generated ObservableProperty when SelectedType changes
        partial void OnSelectedTypeChanged(TextValuePair<string, int> value)
        {
            UpdateListOfBooks();
        }
        
        private void UpdateListOfBooks()
        {
            var source = ViewModel?.Setup?.Books ?? Enumerable.Empty<Book>();
            if (SelectedType == null || SelectedType.Value == 0)
            {
                ListOfBooks = new ObservableCollection<Book>(source);
            }
            else
            {
                var filtered = source.Where(x => (int)x.Type == SelectedType.Value);
                ListOfBooks = new ObservableCollection<Book>(filtered);
            }
        }

        [ObservableProperty]
        private ObservableCollection<TextValuePair<string, int>> typeOptions;

        [ObservableProperty]
        private TextValuePair<string, int> selectedType;

        [ObservableProperty]
        private BookSystemViewModel viewModel;

        [ObservableProperty]
        private BookSetupViewModel bookSetup;

        [ObservableProperty]
        private ObservableCollection<Book> listOfBooks;

        [ObservableProperty]
        private ObservableCollection<BookItem> listOfBookSeries;

    }


}
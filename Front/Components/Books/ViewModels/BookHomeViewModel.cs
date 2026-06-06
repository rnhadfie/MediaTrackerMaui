
using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.BackEnd.Models;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Modals;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using UraniumUI.Dialogs;
using UraniumUI.Extensions;

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

                ListOfBooks = new ObservableCollection<Book>(SelectedType.Value == 0 ? ViewModel.Setup.Books : ViewModel.Setup.Books.Where(x => (int)x.Type == SelectedType.Value) ?? []);

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

    }


}

using MauiApp1.BackEnd.Database;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Modals;
using MauiApp1.Shared;
using Microsoft.EntityFrameworkCore;

namespace MauiApp1.Front.Components.Books;

[QueryProperty(nameof(Add), nameof(Add))]
[QueryProperty(nameof(BookId), "BookId")]
public partial class BookForm: ContentPage
{
	BookSetupViewModel _viewModel;
    Book _book;
    BookController controller;
    private readonly DataContext _dbContext;

    public BookForm(DataContext dataContext)
	{
        
        _dbContext = dataContext;

        InitializeComponent();
        BindingContext = this;
        controller = new BookController(_dbContext);


        #region Setup 
        
        _viewModel = controller.GetBookSetup();

        Book_SeriesCombobox.InputList = _viewModel.Series;
        Book_Genre.InputList = _viewModel.Genre;
        Book_FormatRadioButtons.AddData(_viewModel.Format);
        Book_TypeRadioButtons.AddData(_viewModel.Type);

        #endregion

        _book = new Book()
        {
            Id = -1,
            Title = "",
        };
    }

    private bool add;

    public bool Add
    {
        get => add;
        set
        {
            OnPropertyChanged();
            add = value;
            OnPropertyChanged2();
        }
    }

    // Loading overlay
    Grid _loadingOverlay;
    ActivityIndicator _loadingIndicator;

    void EnsureLoadingOverlay()
    {
        if (_loadingOverlay != null)
            return;

        var original = Content as View;
        var root = new Grid();
        if (original != null)
            root.Children.Add(original);

        var overlay = new Grid
        {
            BackgroundColor = Colors.Black.WithAlpha(0.4f),
            IsVisible = false,
            InputTransparent = false
        };

        var indicator = new ActivityIndicator
        {
            IsRunning = true,
            IsVisible = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Color = Colors.White
        };

        overlay.Children.Add(indicator);
        root.Children.Add(overlay);

        Content = root;

        _loadingOverlay = overlay;
        _loadingIndicator = indicator;
    }

    void ShowLoading()
    {
        EnsureLoadingOverlay();
        _loadingOverlay.IsVisible = true;
        _loadingIndicator.IsRunning = true;
    }

    void HideLoading()
    {
        if (_loadingOverlay == null) return;
        _loadingOverlay.IsVisible = false;
        _loadingIndicator.IsRunning = false;
    }

    private int bookId;
    public int BookId
    {
        get => bookId; set
        {
            OnPropertyChanged();
            bookId = value;
            OnPropertyChanged2();
        }
    }

    private void OnPropertyChanged2()
    {
        if (!add && bookId > 0)
        {
            _book = controller.GetBookInfo(bookId);
            if (_book != null && _book.Id > 0)
            {
                Book_TitleTextField.Value = _book.Title;
                Book_AuthorTextField.Value = _book.Author ?? "";
                Book_ArtistTextField.Value = _book.Artist ?? "";
                Book_PublisherTextField.Value = _book.Publisher ?? "";
                Book_VolumeTextField.Value = _book.Volume.ToString();
                Book_SeriesCombobox.Value = _viewModel.Series.FirstOrDefault(x => x.Value == _book.SeriesId);
                Book_Genre.Value = _viewModel.Genre.FirstOrDefault(x => x.Value == _book.Genre);
                Book_FormatRadioButtons.Value = _book.Format;
                Book_TypeRadioButtons.Value = _book.Type;
                if (_book.Cover != null && _book.Cover.Length > 0)
                {
                    Book_FilePicker.ImageData = _book.Cover;
                }
            }
            else
            {
                _book = new Book()
                {
                    Id = -1,
                    Title = "",
                };
            }
        }
    }

    private async void OnPickFileButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Define file types (optional)
            var options = new PickOptions
            {
                PickerTitle = "Please select a file",
                FileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                    { DevicePlatform.Android, new[] { "image/jpeg", "image/png" } }, // Example for images on Android
                    { DevicePlatform.WinUI, new[] { ".jpg", ".png" } }, // Example for images on Windows
                    })
            };

            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                string fileName = result.FileName;
                string fullPath = result.FullPath;

                // Process the selected file (e.g., read its stream)
                using var stream = await result.OpenReadAsync();
                // ... use the stream ...
            }
        }
        catch (TaskCanceledException tce)
        {
            // User canceled the operation
        }
        catch (Exception ex)
        {
            // Other errors
            //await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        ShowLoading();
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (ArgumentException ex) when (ex.ParamName == "uri" || (ex.Message?.Contains("Ambiguous routes") ?? false))
        {
            // Fallback: prefer popping the navigation stack if possible
            var navigation = Shell.Current?.Navigation;
            if (navigation != null && navigation.NavigationStack.Count > 1)
            {
                await navigation.PopAsync();
            }
            else
            {
                // Last resort: navigate to a known absolute route / root
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
        finally
        {
            HideLoading();
        }
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        //Get and save values

        string title = Book_TitleTextField.Value.ToString();
        if (title.Length <= 0)
        { 
            //Validate 
        }
        string author = Book_AuthorTextField.Value.ToString();
        if (author.Length <= 0)
        {
            //Validate 
        }
        int volume = -1;
        if (Book_VolumeTextField.Value.Length > 0 && !Book_VolumeTextField.Value.IsWhiteSpace())
        { 
            int.TryParse(Book_VolumeTextField.Value, out volume);
        }
        _book.SeriesId = ((TextValuePair<int>)Book_SeriesCombobox.Value).Value;
        _book.Title = title;
        _book.Author = author;
        _book.Publisher = (string)Book_PublisherTextField.Value;
        _book.Artist = (string)Book_ArtistTextField.Value;
        _book.Cover = Book_FilePicker.ImageData ?? [];
        _book.Genre = ((TextValuePair<int>)Book_Genre.Value).Value;
        _book.Format = Book_FormatRadioButtons.Value;
        _book.Type = Book_TypeRadioButtons.Value;
        _book.Volume = volume;

        ShowLoading();
        try
        {
            controller.AddBook(_book);
            await Shell.Current.GoToAsync("..");
        }
        catch (ArgumentException ex) when (ex.ParamName == "uri" || (ex.Message?.Contains("Ambiguous routes") ?? false))
        {
            // Fallback: prefer popping the navigation stack if possible
            var navigation = Shell.Current?.Navigation;
            if (navigation != null && navigation.NavigationStack.Count > 1)
            {
                await navigation.PopAsync();
            }
            else
            {
                // Last resort: navigate to a known absolute route / root
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
        finally
        {
            HideLoading();
        }
    }

    private void SeriesCombobox_SelectionChanged(object sender, EventArgs e)
    {
        int seriesId = ((TextValuePair<int>)Book_SeriesCombobox.Value).Value;
        if (seriesId <= 0)
        {
            return;
        }

        Collection selectedSeries = controller.GetSeriesInfo(seriesId);
        
        this.Book_AuthorTextField.Value = selectedSeries.Author ?? "";
        this.Book_ArtistTextField.Value = selectedSeries.Artist ?? "";
        this.Book_PublisherTextField.Value = selectedSeries.Publisher ?? "";
    }
}
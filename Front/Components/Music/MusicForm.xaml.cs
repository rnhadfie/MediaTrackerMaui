
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.Modals;
using MauiApp1.Shared;
using Microsoft.EntityFrameworkCore;

namespace MauiApp1.Front.Components.Music;

[QueryProperty(nameof(Add), nameof(Add))]
[QueryProperty(nameof(Id), "Id")]
public partial class MusicForm : ContentPage
{
	MusicSetupViewModel _viewModel;
    Cd _cd;
    MusicController controller;
    private readonly DataContext _dbContext;

    public MusicForm(DataContext dataContext)
	{
        
        _dbContext = dataContext;

        InitializeComponent();
        BindingContext = this;
        controller = new MusicController(_dbContext);


        #region Setup 
        
        _viewModel = controller.GetMusicSetup();

        Music_SeriesCombobox.InputList = _viewModel.Collection;
        Music_Genre.InputList = _viewModel.Genres;
        Music_Language.AddData(_viewModel.Langauge);

        #endregion

        _cd = new Cd()
        {
            Id = -1,
            Name = "",
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

    private int id;
    public int Id
    {
        get => id; set
        {
            OnPropertyChanged();
            id = value;
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

    private void OnPropertyChanged2()
    {
        if (!add && id > 0)
        {
            _cd = controller.GetMusicInfo(id);
            if (_cd != null && _cd.Id > 0)
            {
                Music_NameTextField.Value = _cd.Name;
                Music_ArtistTextField.Value = _cd.Artist ?? "";
                Music_SeriesCombobox.Value = _viewModel.Collection.FirstOrDefault(x => x.Value == _cd.Collection);
                Music_Genre.Value = _viewModel.Genres.FirstOrDefault(x => x.Value == _cd.MusicGenre);
                //Music_Language.Value = _viewModel.Langauge
                if (_cd.Cover != null && _cd.Cover.Length > 0)
                {
                    Music_FilePicker.ImageData = _cd.Cover;
                }
            }
            else
            {
                _cd = new Cd()
                {
                    Id = -1,
                    Name = "",
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
        finally
        {
            HideLoading();
        }
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        //Get and save values

        string title = Music_NameTextField.Value.ToString();
        if (title.Length <= 0)
        { 
            //Validate 
        }
        _cd.Collection = ((TextValuePair<int>)Music_SeriesCombobox.Value).Value;
        _cd.Name = title;
        _cd.Artist = (string)Music_ArtistTextField.Value;
        _cd.Cover = Music_FilePicker.ImageData ?? [];
        _cd.Language = Music_Language.Value;
        _cd.MusicGenre = ((TextValuePair<int>)Music_Genre.Value).Value;

        ShowLoading();
        try
        {
            controller.AddMusic(_cd);
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            HideLoading();
        }
    }

    private void SeriesCombobox_SelectionChanged(object sender, EventArgs e)
    {
        int seriesId = ((TextValuePair<int>)Music_SeriesCombobox.Value).Value;
        if (seriesId <= 0)
        {
            return;
        }

        Collection selectedSeries = controller.GetSeriesInfo(seriesId);
        
        this.Music_ArtistTextField.Value = selectedSeries.Artist ?? "";
    }
}
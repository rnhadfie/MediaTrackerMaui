
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Modals;
using MauiApp1.Shared;
using Microsoft.EntityFrameworkCore;

namespace MauiApp1.Front.Components.Other;

[QueryProperty(nameof(Add), nameof(Add))]
[QueryProperty(nameof(Id), "Id")]
public partial class OtherForm: ContentPage
{
	OtherViewModel _viewModel;
    BackEnd.Modals.Other _OtherItem;
    OtherController controller;
    private readonly DataContext _dbContext;

    public OtherForm(DataContext dataContext)
	{
        
        _dbContext = dataContext;

        InitializeComponent();
        BindingContext = this;
        controller = new OtherController(_dbContext);


        #region Setup 
        
        _viewModel = controller.GetOtherSetup();

        Other_SeriesCombobox.InputList = _viewModel.Collection;

        #endregion

        _OtherItem = new BackEnd.Modals.Other()
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
            _OtherItem = controller.GetOtherInfo(id);
            if (_OtherItem != null && _OtherItem.Id > 0)
            {
                Other_TitleTextField.Value = _OtherItem.Name;
                Other_SeriesCombobox.Value = _viewModel.Collection.FirstOrDefault(x => x.Value == _OtherItem.Collection);
                if (_OtherItem.Image != null && _OtherItem.Image.Length > 0)
                {
                    Other_FilePicker.ImageData = _OtherItem.Image;
                }
            }
            else
            {
                _OtherItem = new BackEnd.Modals.Other()
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

        string title = Other_TitleTextField.Value.ToString();
        if (title.Length <= 0)
        { 
            //Validate 
        }
       
        _OtherItem.Collection = ((TextValuePair<int>)Other_SeriesCombobox.Value).Value;
        _OtherItem.Name = title;
        _OtherItem.Image = Other_FilePicker.ImageData ?? [];

        ShowLoading();
        try
        {
            controller.AddOther(_OtherItem);
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            HideLoading();
        }
    }

    private void SeriesCombobox_SelectionChanged(object sender, EventArgs e)
    {
        int seriesId = ((TextValuePair<int>)Other_SeriesCombobox.Value).Value;
        if (seriesId <= 0)
        {
            return;
        }

        Collection selectedSeries = controller.GetSeriesInfo(seriesId);
        
    }
}
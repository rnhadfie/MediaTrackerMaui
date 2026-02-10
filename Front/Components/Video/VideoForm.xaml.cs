using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Hosting;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Front.Components.Video;

[QueryProperty(nameof(Add), nameof(Add))]
[QueryProperty(nameof(VideoId), "VideoId")]
public partial class VideoForm : ContentPage
{
    //Vide _viewModel;
    VideoController controller;
    VideoSetupViewModel _viewModel;
    private MauiApp1.Service.Modals.Video _Video;
    private readonly DataContext _dbContext;
    public VideoForm(DataContext dataContext)
	{
        _dbContext = dataContext;
        controller = new VideoController(_dbContext);
        _viewModel = controller.GetVideoSetup();

        InitializeComponent();

        Video_SeriesCombobox.InputList = _viewModel.Series;
        Video_CategoryCombobox.InputList = _viewModel.Category;
        Video_FormatRadioButtons.AddData(_viewModel.VideoFormat);
        Video_TypeRadioButtons.AddData(_viewModel.VideoType);

        _Video = new MauiApp1.Service.Modals.Video()
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

    private int videoId;
    public int VideoId
    {
        get => videoId; set
        {
            OnPropertyChanged();
            videoId = value;
            OnPropertyChanged2();
        }
    }

    private void OnPropertyChanged2()
    {
        if (!add && videoId > 0)
        {
            _Video = controller.GetVideoInfo(videoId);
            if (_Video != null && _Video.SeriesId > 0)
            {
                Video_TitleTextField.Value = _Video.Name;
                Video_SeriesCombobox.Value = _viewModel.Series.FirstOrDefault(x => x.Value == _Video.SeriesId);
                Video_FormatRadioButtons.Value = _Video.VideoFormat;
                Video_TypeRadioButtons.Value = _Video.Type;
            }
            else
            {
                _Video = new MauiApp1.Service.Modals.Video()
                {
                    Id = -1,
                    Name = "",
                };
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MainPage");
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

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        //Get and save values
        
        string title = Video_TitleTextField.Value.ToString();
        if (title.Length <= 0)
        {
            //Validate 
        }

        int volume = -1;
        if (Video_VolumeTextField.Value.Length > 0 && !Video_VolumeTextField.Value.IsWhiteSpace())
        {
            int.TryParse(Video_VolumeTextField.Value, out volume);
        }
        MauiApp1.Service.Modals.Video newVideo = new MauiApp1.Service.Modals.Video
        {
           Id = -1,
           SeriesId = ((TextValuePair<int>)Video_SeriesCombobox.Value).Value,
            Name = Video_TitleTextField.Value,
           Category = ((TextValuePair<int>)Video_CategoryCombobox.Value).Value,
           VideoFormat = Video_FormatRadioButtons.Value,
           Type = Video_TypeRadioButtons.Value,
           Cover = Video_FilePicker.ImageData
        };


        controller.AddVideo(newVideo);

        await Shell.Current.GoToAsync("///MainPage");
    }

    private void Video_CategoryCombobox_SelectionChanged(object sender, EventArgs e)
    {

    }

    private void Video_SeriesCombobox_SelectionChanged(object sender, EventArgs e)
    {

    }
}
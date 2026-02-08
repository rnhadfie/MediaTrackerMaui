
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using Microsoft.EntityFrameworkCore;

namespace MauiApp1.Components.Books;

public partial class AddBook
{
	BookSetupViewModel _viewModel;
    BookController controller;
    private readonly DataContext _dbContext;
    public AddBook(DataContext dataContext)
	{
        _dbContext = dataContext;

        InitializeComponent();
        controller = new BookController(_dbContext);

        _viewModel = controller.GetBookSetup();

        SeriesCombobox.InputList = _viewModel.ListOfSeries;
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
        await Shell.Current.GoToAsync("///MainPage");
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        //Get and save values

        string title = TitleTextField.Value.ToString();
        if (title.Length <= 0)
        { 
            //Validate 
        }
        string author = AuthorTextField.Value.ToString();
        if (author.Length <= 0)
        {
            //Validate 
        }
        int volume = -1;
        if (VolumeTextField.Value.Length > 0 && !VolumeTextField.Value.IsWhiteSpace())
        { 
            int.TryParse(VolumeTextField.Value, out volume);
        }
        Book book = new Book {
            SeriesId = ((TextValuePair<int>)SeriesCombobox.Value).Value,
            Title = title,
            Author = author,
            Publisher = (string)publisherTextField.Value,
            Artist = (string)ArtistTextField.Value,
            Cover = bookFilePicker.ImageData,
            volume = volume
        };

        controller.AddBook(book);

        await Shell.Current.GoToAsync("///MainPage");
    }

    private void SeriesCombobox_SelectionChanged(object sender, EventArgs e)
    {
        int seriesId = ((TextValuePair<int>)SeriesCombobox.Value).Value;
        if (seriesId <= 0)
        {
            return;
        }

        Series selectedSeries = controller.GetSeriesInfo(seriesId);
        
        this.AuthorTextField.Value = selectedSeries.Author;
        this.ArtistTextField.Value = selectedSeries.Artist;
        this.publisherTextField.Value = selectedSeries.Publisher;
        this.AuthorTextField.Value = selectedSeries.Author;
    }
}
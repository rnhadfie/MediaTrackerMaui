
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Service.Modals;

namespace MauiApp1.Components.Books;

public partial class AddBook
{
	BookSetupViewModel _viewModel;
    BookController controller;
    public AddBook()
	{
		InitializeComponent();
        controller = new BookController();

        _viewModel = controller.GetBookSetup();
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
        if (title.Length > 0)
        { 
            //Validate 
        }
        string author = AuthorTextField.Value.ToString();
        if (author.Length > 0)
        {
            //Validate 
        }

        Book book = new Book {
            Title = title,
            Author = author,
            Publisher = (string)publisherTextField.Value,
            Artist = (string)ArtistTextField.Value,
            volume = int.Parse((string)VolumeTextField.Value)
        };

        await Shell.Current.GoToAsync("///MainPage");
    }
}
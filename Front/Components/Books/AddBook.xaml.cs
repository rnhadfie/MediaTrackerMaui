
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Front.Components.Shared;

namespace MauiApp1.Components.Books;

public partial class AddBook : ContentPage
{
	BookSetupViewModel _viewModel;
    BookController controller;
    public AddBook()
	{
		InitializeComponent();
        controller = new BookController();

        _viewModel = controller.GetBookSetup();

        SetupForm();
    }

    public void SetupForm()
    {
        
        //Dropdown
        HorizontalStackLayout seriesLayout = new HorizontalStackLayout();
        seriesLayout.HorizontalOptions = LayoutOptions.Start;
        seriesLayout.Add(new Label
        {
            Text = "Series",
            WidthRequest = 50,
            HorizontalOptions = LayoutOptions.Start,
            HorizontalTextAlignment = TextAlignment.End

        });
        seriesLayout.Add(new Picker
        {
            ItemDisplayBinding = new Binding("Text"),
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 250,
            ItemsSource = _viewModel.ListOfSeries
        });

        var page = this.Content.FindByName<VerticalStackLayout>("AddBookForm");
        page.Add(seriesLayout);

        page.Add(new TextField("Title"));
        page.Add(new TextField("Author"));
        page.Add(new TextField("Artist", -1,-1, null, "Artist or co-author"));
        page.Add(new TextField("Volume"));
        page.Add(new Checkbox());


        page.Add(new Button
        {
            Text = "Add Book",
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest = 200,
        });
        //
        /*
         
        </HorizontalStackLayout>
        <HorizontalStackLayout>
            <Label Text="Volume" WidthRequest="100" HorizontalOptions="End"/>
            <Entry
                Completed="OnEntryCompleted"
                Keyboard="Numeric"
                MaxLength="50" />
        </HorizontalStackLayout>
        <HorizontalStackLayout>
            <CheckBox />
            <Label Text="Completed" WidthRequest="100" HorizontalOptions="End" VerticalOptions="Center"/>
        </HorizontalStackLayout>
        <HorizontalStackLayout>
            <Label Text="Cover Image" />
        </HorizontalStackLayout>
        <Button
            Text="Add Book"></Button>
         */
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
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnNavigatedTo(object sender, NavigatedToEventArgs args)
    {
        // Invoked when the page has been navigated to
        Page? previousPage = args.PreviousPage;
        NavigationType navigationType = args.NavigationType;
    }
}
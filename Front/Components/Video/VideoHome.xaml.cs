using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.Front.Components.Videos.ViewModels;
using System.ComponentModel;

namespace MauiApp1.Front.Components.Videos;

public partial class VideoHome : ContentPage
{
    private readonly VideoHomeViewModel _vm;

    public VideoHome(VideoHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _vm = viewModel;

        //VideoTypeDropDown.ItemDisplayBinding = new Binding("Text");
    }


    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        // Prompt the user to choose between adding an item or a series
        var choice = await Application.Current.MainPage.DisplayActionSheet("Add", "Cancel", null, "Item", "Series");
        if (string.IsNullOrEmpty(choice) || choice == "Cancel") return;
        if (choice == "Item")
        {
            await Shell.Current.GoToAsync($"/VideoForm?id=0&edit=true");
        }
        else if (choice == "Series")
        {
            await Shell.Current.GoToAsync("/VideoSeriesForm?edit=true");
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn)
            {
                var video = (MauiApp1.BackEnd.Models.Video.Video)btn.CommandParameter;
                var action = await Application.Current.MainPage.DisplayActionSheet("Actions", "Cancel", null, "View", "Edit", "Delete");
                if (action == "Cancel" || string.IsNullOrEmpty(action)) return;
                switch (action)
                {
                    case "View":
                        await Shell.Current.GoToAsync($"/VideoForm?id={video.Id}&edit=false");
                        break;
                    case "Edit":
                        await Shell.Current.GoToAsync($"/VideoForm?id={video.Id}&edit=true");
                        break;
                    case "Delete":
                        var confirm = await Application.Current.MainPage.DisplayAlert("Confirm", "Delete this item?", "Yes", "No");
                        if (confirm)
                        {
                            var controller = new BackEnd.Controllers.ViewModels.VideoController();
                            var result = await controller.DeleteVideoAsync(video);
                            if (result > 0)
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Item deleted: {video.Series}", "OK");
                            else
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Failed to delete: {video.Series}", "OK");

                            // Refresh list
                            await _vm.LoadSetupAsync();
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void ShowDialog_Clicked(object sender, EventArgs e)
    {
        // Same choice as the top Add button - allow adding item or series
        var choice = await Application.Current.MainPage.DisplayActionSheet("Add", "Cancel", null, "Item", "Series");
        if (string.IsNullOrEmpty(choice) || choice == "Cancel") return;
        if (choice == "Item")
        {
            await Shell.Current.GoToAsync($"/VideoForm?id=0&edit=true");
        }
        else if (choice == "Series")
        {
            await Shell.Current.GoToAsync("/VideoSeriesForm?edit=true");
        }
    }

    private async void OpenSeriesManager_Clicked(object sender, EventArgs e)
    {
        // Redirect to the Videos home route which contains the embedded series list
        await Shell.Current.GoToAsync("videohome");
    }
}

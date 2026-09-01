using LiveChartsCore.Geo;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using MauiApp1.Controllers;
using MauiApp1.Front.Components.Books;
using System.Collections.ObjectModel;

namespace MauiApp1.Front.Components.Videos;

public partial class VideoSeriesList : ContentView
{
    private BackEnd.Controllers.ViewModels.VideoController _videoController;

    public VideoSeriesList()
    {
        InitializeComponent();
        _videoController = new BackEnd.Controllers.ViewModels.VideoController();
    }


    private bool filtering = false;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable<VideoItem>),
        typeof(VideoSeriesList),
        default(IEnumerable<VideoItem>),
        propertyChanged: OnItemsSourceChanged);

    public IEnumerable<VideoItem> ItemsSource
    {
        get => (IEnumerable<VideoItem>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IEnumerable<VideoItem> OriginalVideos { get; set; }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is VideoSeriesList control)
        {
            control.OnItemsSourceChanged((IEnumerable<VideoItem>)oldValue, (IEnumerable<VideoItem>)newValue);
        }
    }

    private void OnItemsSourceChanged(IEnumerable<VideoItem> oldValue, IEnumerable<VideoItem> newValue)
    {
        // If the inner list view uses ItemsSource binding, update it directly
        if (newValue == null)
        {
            // Clear
            VideoSeriesListDataGrid.ItemsSource = null;
            return;
        }

        VideoSeriesListDataGrid.ItemsSource = (ObservableCollection<VideoItem>)newValue;
        if (!filtering)
        {
            OriginalVideos = (ObservableCollection<VideoItem>)newValue;
        }
    }

    public static readonly BindableProperty EmptyViewTextProperty =
        BindableProperty.Create(
            nameof(EmptyViewText),
            typeof(string),
            typeof(VideoSeriesList),
            default(string),
            BindingMode.OneWay);

    public string EmptyViewText
    {
        get => (string)GetValue(EmptyViewTextProperty);
        set => SetValue(EmptyViewTextProperty, value);
    }

    public static readonly BindableProperty SearchTextProperty =
        BindableProperty.Create(
            nameof(SearchText),
            typeof(string),
            typeof(BookList),
            "",
            BindingMode.TwoWay);

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    private void TextField_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            VideoSeriesListDataGrid.ItemsSource = ItemsSource.Where(x => x.DiscTitle.Contains(SearchText)).ToList();
        }
    }

    private void TextField_TextChanged(object sender, TextChangedEventArgs e)
    {
        filtering = !string.IsNullOrWhiteSpace(e.NewTextValue);
        if (e.NewTextValue != null)
        {
            filtering = true;
            SearchText = e.NewTextValue;
            List<VideoItem> fitleredList = (List<VideoItem>)OriginalVideos;
            VideoSeriesListDataGrid.ItemsSource = fitleredList.FindAll(x => x.DiscTitle.ToLower().Contains(SearchText.ToLower())).ToList();
        }

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn)
            {
                VideoItem videoSeries = (VideoItem)btn.CommandParameter;

                // Show a bottom-sheet style action sheet using the platform ActionSheet
                // This is a lightweight bottom sheet alternative that works across MAUI
                var action = await Application.Current.MainPage.DisplayActionSheet("Actions", "Cancel", null, "View", "Edit", "Delete");

                if (action == "Cancel" || string.IsNullOrEmpty(action))
                    return;

                // Handle actions - these are placeholders for integration with navigation/commands
                switch (action)
                {
                    case "View":
                        await Shell.Current.GoToAsync($"/BookForm?id={videoSeries.Id}&edit=false");
                        break;
                    case "Edit":
                        await Shell.Current.GoToAsync($"/BookForm?id={videoSeries.Id}&edit=true");
                        break;
                    case "Delete":
                        var confirm = await Application.Current.MainPage.DisplayAlert("Confirm", "Delete this item?", "Yes", "No");
                        if (confirm)
                        {
                            var result = await _videoController.DeleteVideoSeriesAsync(videoSeries);
                            if (result > 0)
                            {
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Item deleted: {videoSeries.DiscTitle}", "OK");
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Failed to delete: {videoSeries.DiscTitle}", "OK");
                            }
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

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("/VideoSeriesForm?edit=true");
    }
}

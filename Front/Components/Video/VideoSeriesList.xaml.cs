using MauiApp1.BackEnd.Models.Video;
using MauiApp1.BackEnd.Controllers.ViewModels;
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
        typeof(IEnumerable<VideoSeries>),
        typeof(VideoSeriesList),
        default(IEnumerable<VideoSeries>),
        propertyChanged: OnItemsSourceChanged);

    public IEnumerable<VideoSeries> ItemsSource
    {
        get => (IEnumerable<VideoSeries>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IEnumerable<VideoSeries> OriginalSeries { get; set; }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is VideoSeriesList control)
        {
            control.OnItemsSourceChanged((IEnumerable<VideoSeries>)oldValue, (IEnumerable<VideoSeries>)newValue);
        }
    }

    private void OnItemsSourceChanged(IEnumerable<VideoSeries> oldValue, IEnumerable<VideoSeries> newValue)
    {
        if (newValue == null)
        {
            VideoSeriesListDataGrid.ItemsSource = null;
            return;
        }

        VideoSeriesListDataGrid.ItemsSource = (ObservableCollection<VideoSeries>)newValue;
        if (!filtering)
        {
            OriginalSeries = (ObservableCollection<VideoSeries>)newValue;
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
            typeof(VideoSeriesList),
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
            VideoSeriesListDataGrid.ItemsSource = ItemsSource.Where(x => x.Title.Contains(SearchText)).ToList();
        }
    }

    private void TextField_TextChanged(object sender, TextChangedEventArgs e)
    {
        filtering = !string.IsNullOrWhiteSpace(e.NewTextValue);
        if (e.NewTextValue != null)
        {
            filtering = true;
            SearchText = e.NewTextValue;
            List<VideoSeries> fitleredList = (List<VideoSeries>)OriginalSeries;
            VideoSeriesListDataGrid.ItemsSource = fitleredList.FindAll(x => x.Title.ToLower().Contains(SearchText.ToLower())).ToList();
        }

    }

    public async Task<bool> LoadSeriesAsync()
    {
        OriginalSeries = await _videoController.GetAllVideoSeriesAsync();
        ItemsSource = OriginalSeries;
        VideoSeriesListDataGrid.ItemsSource = (System.Collections.IList)OriginalSeries;
        return true;
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("/VideoSeriesForm?edit=true");
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn)
            {
                var series = (VideoSeries)btn.CommandParameter;
                var action = await Application.Current.MainPage.DisplayActionSheet("Actions", "Cancel", null, "View", "Edit", "Delete");
                if (action == "Cancel" || string.IsNullOrEmpty(action)) return;
                switch (action)
                {
                    case "View":
                        await Shell.Current.GoToAsync($"/VideoSeriesForm?id={series.Id}&edit=false");
                        break;
                    case "Edit":
                        await Shell.Current.GoToAsync($"/VideoSeriesForm?id={series.Id}&edit=true");
                        break;
                    case "Delete":
                        var confirm = await Application.Current.MainPage.DisplayAlert("Confirm", "Delete this item?", "Yes", "No");
                        if (confirm)
                        {
                            var result = await _videoController.DeleteVideoSeriesAsync(series);
                            if (result > 0)
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Item deleted: {series.Title}", "OK");
                            else
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Failed to delete: {series.Title}", "OK");
                            await LoadSeriesAsync();
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
}

using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Collections;


public partial class CollectionView : ContentPage
{

    private CollectionController _seriesViewController;
    private readonly DataContext _dbContext;
    private List<Modals.Collection> _TotalSeriesList;
    private List<DisplayViewItem> _SeriesList;
    private const double CellSize = 160;
    private string _TextFilter = "";
    private MediaDataType _TypeFilter = MediaDataType.All;
    private CollectionStatus _StatusFilter = CollectionStatus.All;

    public CollectionView(DataContext dataContext)
	{
		InitializeComponent();
        _dbContext = dataContext;

        _seriesViewController = new CollectionController(dataContext);
        _TotalSeriesList = _seriesViewController.GetSeriesList();

        _SeriesList = GetDisplayList();

        var _seriesTypeList = _seriesViewController.GetSeriesSetup();
        SeriesView_TypeComboBox.InputList = _seriesTypeList.ListOfMediaTypes;
        SeriesView_StatusComboBox.InputList = _seriesTypeList.ListOfSeriesStatus;

        UpdateGrid();

    }

    private List<DisplayViewItem> GetDisplayList()
    {
        _TotalSeriesList = _seriesViewController.GetSeriesList();
        //_TotalSeriesList.RemoveAt(0);

        var list = _TotalSeriesList.FindAll(x =>
        {
            bool textMatch = string.IsNullOrEmpty(_TextFilter) || x.Title.Contains(_TextFilter, StringComparison.OrdinalIgnoreCase);
            bool typeMatch = x.type == MediaDataType.All || x.type == MediaDataType.Collection || x.type == _TypeFilter;
            bool statusMatch = _StatusFilter == CollectionStatus.All || x.CollectionStatus == _StatusFilter;
            return textMatch && typeMatch && statusMatch;
        });

        return list.Select(x => new DisplayViewItem
        {
            Id = x.SeriesId,
            Name = x.Title,
            Type = x.type,
            Cover = []
        }).ToList();
    }

    private void OnGridSizeChanged(object sender, EventArgs e)
    {
        UpdateGrid();
    }

    private async void OnOpenMenu(object? sender, EventArgs e)
    {
        ShowLoading();
        try
        {
            await Shell.Current.GoToAsync($"{nameof(CollectionForm)}?Add={true}&SeriesId={-1}");
        }
        finally
        {
            HideLoading();
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
   
    private void UpdateGrid()
    {
        SeriesGridView.InputList = _SeriesList;
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        await HideElementWithSlideAnimation(!FilterDrawer.IsVisible);
    }

    private async Task HideElementWithSlideAnimation(bool show)
    {
        FilterDrawer.IsVisible = show;

        FilterDrawer.TranslationY = 0;
        FilterDrawer.Opacity = 1;
    }

    private async void FilterButton_Clicked(object sender, EventArgs e)
    {
        _TextFilter = SeriesView_Search.Value;
        var typeValue = SeriesView_TypeComboBox.Value;
        var statusValue = SeriesView_StatusComboBox.Value;

        var type = (typeValue != null) ? (TextValuePair<int>)typeValue : null;
        var status = statusValue != null ? (TextValuePair<int>)statusValue : null;

        _TypeFilter = type != null ? (MediaDataType)type.Value : MediaDataType.All;
        _StatusFilter = status != null ? (CollectionStatus)status.Value : CollectionStatus.All;
        await HideElementWithSlideAnimation(false);

        _SeriesList = GetDisplayList();

        UpdateGrid();
    }
}
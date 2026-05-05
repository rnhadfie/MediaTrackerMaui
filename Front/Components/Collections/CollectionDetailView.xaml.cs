using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;

namespace MauiApp1.Front.Components.Collections;


[QueryProperty(nameof(SeriesId), "SeriesId")]
public partial class CollectionDetailView : ContentPage
{
    CollectionController controller;
    private readonly DataContext _dbContext;
    private List<DisplayViewItem> _CollectionView;
    private int CellSize = 160;

    public CollectionDetailView(DataContext dataContext)
    {
        _dbContext = dataContext;

        InitializeComponent();
        controller = new CollectionController(_dbContext);
        _CollectionView = new List<DisplayViewItem>();
    }

    private int seriesId;
    public int SeriesId
    {
        get => seriesId; set
        {
            OnPropertyChanged();
            seriesId = value;
            OnPropertyChanged2();
        }
    }

    private void OnPropertyChanged2()
    {
        if (seriesId > 0)
        {

            _CollectionView = controller.GetSeriesItems(seriesId);
            CollectionsDetailView.InputList = _CollectionView;
            UpdateGrid();
        }
    }

    private void UpdateGrid()
    {

        CollectionsDetailView.InputList = _CollectionView;
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
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
}
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Video;

[QueryProperty(nameof(VideoType), nameof(VideoType))]
[QueryProperty(nameof(VideoGroup), nameof(VideoGroup))]
public partial class VideoDetailView : ContentPage
{

    private VideoController _VideoController;
    private readonly DataContext _dbContext;
    private List<DisplayViewItem> _VideoList;
    private List<Modals.Video> _AllVideos;
    private bool _TypeLoaded = false;
    private bool _FormatLoaded = false;

    private string _TextFilter;
    private VideoType? _TypeFilter = null;
    private VideoGroup? _FormatFilter = null;
    private Genre? _GenreFilter = null;


    public VideoDetailView(DataContext dataContext)
    {
        _dbContext = dataContext;
        _VideoController = new VideoController(_dbContext);
        InitializeComponent();
        BindingContext = this;

        var setup = _VideoController.GetVideoSetup();

        VideoDetailView_Type.InputList = setup.VideoType;
        //VideoDetailView_Genre.InputList = setup.Genre;
        VideoDetailView_Group.InputList = setup.Category;
    }

    private int _VideoGroup;

    public int VideoGroup
    {
        get => _VideoGroup;
        set
        {
            OnPropertyChanged();
            _VideoGroup = value;
            OnLoad();
            _FormatLoaded = true;
        }
    }

    private int _VideoType;
    public int VideoType
    {
        get => _VideoType; set
        {
            OnPropertyChanged();
            _VideoType = value;
            OnLoad();
            _TypeLoaded = true;
        }
    }

    private void OnLoad()
    {
        if (_TypeLoaded && _FormatLoaded)
        {
            return;
        }
        VideoFitler filter = new VideoFitler() { Type = -1 };
        if (_VideoType > 0)
        {
            filter.Type = _VideoType;
        }


        if (_VideoGroup > 0)
        {
            filter.Group = _VideoGroup;
        }

        if ((_TypeLoaded && !_FormatLoaded) || (!_TypeLoaded && _FormatLoaded))
        {

            _AllVideos = _VideoController.GetAllVideos(filter);
            _VideoList = GetDisplayList();
            UpdateGrid();
        }
    }

    private void OnGridSizeChanged(object sender, EventArgs e)
    {
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        VideoGridView.InputList = _VideoList;
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
        
        _TextFilter = VideoDetailView_Search.Value;
        var typeValue = VideoDetailView_Type.Value;
        var formatValue = VideoDetailView_Group.Value;
        var genreValue = BookDetailView_Genre.Value;

        var type = (typeValue != null) ? (TextValuePair<int>)typeValue : null;
        var format = formatValue != null ? (TextValuePair<int>)formatValue : null;
        var genre = genreValue != null ? (TextValuePair<int>)genreValue : null;

        _TypeFilter = type != null ? (VideoType)type.Value : null;
        _FormatFilter = format != null ? (VideoGroup)format.Value : null;
        _GenreFilter = genre != null ? (Genre)genre.Value : null;
        await HideElementWithSlideAnimation(false);

        _VideoList = GetDisplayList();
        ShowLoading();
        try
        {
            UpdateGrid();
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

    private List<DisplayViewItem> GetDisplayList()
    {


        var list = _AllVideos.FindAll(x =>
        {
            bool textMatch = string.IsNullOrEmpty(_TextFilter)
                            || x.Name.Contains(_TextFilter, StringComparison.OrdinalIgnoreCase);

            bool typeMatch = _TypeFilter == null || x.Type == (int)_TypeFilter.Value;
            bool formatMatch = _FormatFilter == null || x.Category == (int)_FormatFilter.Value;
             //bool genreMatch = _GenreFilter == null;
            return textMatch; //&& formatMatch && genreMatch && typeMatch;
        });

        return list.Select(x => new DisplayViewItem
        {
            Id = x.Id,
            Name = x.Name,
            Type = MediaDataType.Video,
            Cover = x.Cover
        }).ToList();
    }
}
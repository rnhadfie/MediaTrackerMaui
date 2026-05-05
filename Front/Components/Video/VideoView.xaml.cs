using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Front.Components.Books;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Video;

public partial class VideoView : ContentPage
{

    VideoSetupViewModel _viewModel;
    List<Modals.Video> _Videos;
    private VideoController controller;
    private ScrollViewController _scrollViewController;
    private readonly DataContext _dbContext;
    public VideoView(DataContext dataContext)
    {
        InitializeComponent();
        _dbContext = dataContext;
        controller = new VideoController(_dbContext);
        _scrollViewController = new ScrollViewController(_dbContext);


        VideoFitler filter = new VideoFitler()
        {
            Type = null,
            Take = null,
            Genre = null,
            Series = null,
            Group = null
        };

        #region Setup 
        _Videos = controller.GetAllVideos(filter);
        _viewModel = controller.GetVideoSetup();

        VideoView_All.Source = GetListOfDisplayViewItems(_Videos, 10);
        VideoView_All.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={-1}";

        List<Modals.Video> animeList = _Videos.Where< Modals.Video>(x => x.Category == (int)VideoGroup.Anime).ToList();
        VideoView_Anime.Source = GetListOfDisplayViewItems(animeList, 10);
        VideoView_Anime.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoGroup.Anime}";

        List<Modals.Video> liveActionList = _Videos.Where<Modals.Video>(x => x.Type == (int)VideoGroup.LiveAction).ToList();
        VideoView_LiveAction.Source = GetListOfDisplayViewItems(liveActionList, 10);
        VideoView_LiveAction.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoGroup.LiveAction}";

        List<Modals.Video> westernAnimationList = _Videos.Where<Modals.Video>(x => x.Type == (int)VideoGroup.WesternAnimation).ToList();
        VideoView_WesternAnimation.Source = GetListOfDisplayViewItems(westernAnimationList, 10);
        VideoView_WesternAnimation.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoGroup.WesternAnimation}";

        List<Modals.Video> movieList = _Videos.Where<Modals.Video>(x => x.Type == (int)VideoType.Movie).ToList();
        VideoView_Movies.Source = GetListOfDisplayViewItems(westernAnimationList, 10);
        VideoView_Movies.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoType.Movie}";

        List<Modals.Video> tvShowList = _Videos.Where<Modals.Video>(x => x.Type == (int)VideoType.tvShow).ToList();
        VideoView_TvShows.Source = GetListOfDisplayViewItems(westernAnimationList, 10);
        VideoView_TvShows.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoType.tvShow}";


        VideoView_Series.Source = _scrollViewController.GetSeriesList().FindAll(x => x.Type == MediaDataType.All || x.Type == MediaDataType.Video);
        //BookView_Series.LocationText = $"{nameof(BookDetailView)}?BookType={BookType.Manga}&BookFormat={-1}";

        #endregion
    }


    private List<DisplayViewItem> GetListOfDisplayViewItems(List<Modals.Video> videos, int take)
    {

        List<Modals.Video> finalList = videos.Take<Modals.Video>(10).ToList();
        return finalList.Select(x =>
        {
            return new DisplayViewItem
            {
                Type = MediaDataType.Video,
                Cover = x.Cover,
                Id = x.Id,
                Name = x.Name
            };
        }).ToList<DisplayViewItem>();

    }

    private async void OnOpenMenu(object? sender, EventArgs e)
    {
        ShowLoading();
        try
        {
            await Shell.Current.GoToAsync($"{nameof(BookForm)}?Add={true}&BookId={-1}");
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
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Components.Books;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Front.Components.Books;
using MauiApp1.Service.Modals;
using System.Linq;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Front.Components.Video;

public partial class VideoView : ContentPage
{

    VideoSetupViewModel _viewModel;
    List<Service.Modals.Video> _Videos;
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

        List<Service.Modals.Video> animeList = _Videos.Where< Service.Modals.Video>(x => x.Category == (int)VideoGroup.Anime).ToList();
        VideoView_Anime.Source = GetListOfDisplayViewItems(animeList, 10);
        VideoView_Anime.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoGroup.Anime}";

        List<Service.Modals.Video> liveActionList = _Videos.Where<Service.Modals.Video>(x => x.Type == (int)VideoGroup.LiveAction).ToList();
        VideoView_LiveAction.Source = GetListOfDisplayViewItems(liveActionList, 10);
        VideoView_LiveAction.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoGroup.LiveAction}";

        List<Service.Modals.Video> westernAnimationList = _Videos.Where<Service.Modals.Video>(x => x.Type == (int)VideoGroup.WesternAnimation).ToList();
        VideoView_WesternAnimation.Source = GetListOfDisplayViewItems(westernAnimationList, 10);
        VideoView_WesternAnimation.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoGroup.WesternAnimation}";

        List<Service.Modals.Video> movieList = _Videos.Where<Service.Modals.Video>(x => x.Type == (int)VideoType.Movie).ToList();
        VideoView_Movies.Source = GetListOfDisplayViewItems(westernAnimationList, 10);
        VideoView_Movies.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoType.Movie}";

        List<Service.Modals.Video> tvShowList = _Videos.Where<Service.Modals.Video>(x => x.Type == (int)VideoType.tvShow).ToList();
        VideoView_TvShows.Source = GetListOfDisplayViewItems(westernAnimationList, 10);
        VideoView_TvShows.LocationText = $"{nameof(VideoDetailView)}?VideoType={-1}&VideoGroup={(int)VideoType.tvShow}";


        VideoView_Series.Source = _scrollViewController.GetSeriesList().FindAll(x => x.Type == MediaDataType.All || x.Type == MediaDataType.Video);
        //BookView_Series.LocationText = $"{nameof(BookDetailView)}?BookType={BookType.Manga}&BookFormat={-1}";

        #endregion
    }


    private List<DisplayViewItem> GetListOfDisplayViewItems(List<Service.Modals.Video> videos, int take)
    {

        List<Service.Modals.Video> finalList = videos.Take<Service.Modals.Video>(10).ToList();
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
        await Shell.Current.GoToAsync($"{nameof(BookForm)}?Add={true}&BookId={-1}");
    }
}
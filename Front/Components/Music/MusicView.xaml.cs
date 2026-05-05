
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Modals;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Music;

public partial class MusicView : ContentPage
{
    MusicSetupViewModel _viewModel;
    List<Cd> _cds;
    private MusicController controller;
    private ScrollViewController _scrollViewController;
    private readonly DataContext _dbContext;
    public MusicView(DataContext dataContext)
    {
        InitializeComponent();
        _dbContext = dataContext;
        controller = new MusicController(_dbContext);
        _scrollViewController = new ScrollViewController(_dbContext);


        #region Setup 
        _cds = controller.GetMusicItems(-1);
        _viewModel = controller.GetMusicSetup();

        MusicView_All.Source = GetListOfDisplayViewItems(_cds, 10);
        MusicView_All.LocationText = $"{nameof(MusicDetailView)}";



        MusicView_Series.Source = _scrollViewController.GetSeriesList().FindAll(x=>x.Type == MediaDataType.All || x.Type == MediaDataType.Cd);
       

        #endregion
    }


    private List<DisplayViewItem> GetListOfDisplayViewItems(List<Cd> cds, int take)
    {

        List<Cd> finalList = cds.Take<Cd>(10).ToList();
        return finalList.Select(x =>
        {
            return new DisplayViewItem
            {
                Type = MediaDataType.Cd,
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
            await Shell.Current.GoToAsync($"{nameof(MusicForm)}?Add={true}&Id={-1}");
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
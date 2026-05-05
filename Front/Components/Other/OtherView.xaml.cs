
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Other;

public partial class OtherView : ContentPage
{
    OtherViewModel _viewModel;
    List<BackEnd.Modals.Other> _OtherItems;
    private OtherController controller;
    private ScrollViewController _scrollViewController;
    private readonly DataContext _dbContext;
    public OtherView(DataContext dataContext)
    {
        InitializeComponent();
        _dbContext = dataContext;
        controller = new OtherController(_dbContext);
        _scrollViewController = new ScrollViewController(_dbContext);

       
        #region Setup 
        _OtherItems = controller.GetAllOtehrItems();
        _viewModel = controller.GetOtherSetup();

        OtherView_All.Source = GetListOfDisplayViewItems(_OtherItems, 10);
        OtherView_All.LocationText = $"{nameof(OtherDetailView)}?BookType={-1}&BookFormat={-1}";



        OtherView_Series.Source = _scrollViewController.GetSeriesList().FindAll(x=>x.Type == MediaDataType.All || x.Type == MediaDataType.Book);
        //BookView_Series.LocationText = $"{nameof(BookDetailView)}?BookType={BookType.Manga}&BookFormat={-1}";

        #endregion
    }


    private List<DisplayViewItem> GetListOfDisplayViewItems(List<BackEnd.Modals.Other> otherItems, int take)
    {

        List<BackEnd.Modals.Other> finalList = otherItems.Take<BackEnd.Modals.Other>(10).ToList();
        return finalList.Select(x =>
        {
            return new DisplayViewItem
            {
                Type = MediaDataType.Book,
                Cover = x.Image,
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
            await Shell.Current.GoToAsync($"{nameof(OtherForm)}?Add={true}&Id={-1}");
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
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.Components;
using MauiApp1.Components.Menu;
using MauiApp1.Front.Components.Books;
using MauiApp1.Front.Components.Video;
using System.Threading;
using CollectionView = MauiApp1.Front.Components.Collections.CollectionView;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private ScrollViewController _scrollViewController;
        private readonly DataContext _dbContext;
        public MainPage(DataContext dataContext)
        {
            InitializeComponent();
            _dbContext = dataContext;

            _scrollViewController = new ScrollViewController(dataContext);

                SeriesScrollView.Source = _scrollViewController.GetSeriesList(10);
                SeriesScrollView.LocationText = nameof(CollectionView);

                BookScrollView.Source = _scrollViewController.GetBookList(10);
                BookScrollView.LocationText = nameof(BookView);
                
                VideoScrollView.Source = _scrollViewController.GetVideoList(10);
                VideoScrollView.LocationText = nameof(VideoView);
        }


        private void OnOpenMenu(object? sender, EventArgs e)
        {
            this.ShowPopup(new PopupMenuPage(_dbContext));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshData();
        }

        private void RefreshData()
        {
            SeriesScrollView.Source.Clear();
            SeriesScrollView.Source = _scrollViewController.GetSeriesList(10);

            BookScrollView.Source.Clear();
            BookScrollView.Source = _scrollViewController.GetBookList(10);

            VideoScrollView.Source.Clear();
            VideoScrollView.Source = _scrollViewController.GetVideoList(10);
        }
    }
}

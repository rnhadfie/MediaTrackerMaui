using CommunityToolkit.Maui.Extensions;
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.Components;

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
                BookScrollView.Source = _scrollViewController.GetBookList(10);
                VideoScrollView.Source = _scrollViewController.GetVideoList(10);
        }


        private void OnOpenMenu(object? sender, EventArgs e)
        {
            this.ShowPopup(new PopupMenuPage(_dbContext));
        }
    }
}

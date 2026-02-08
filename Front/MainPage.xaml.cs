using CommunityToolkit.Maui.Extensions;
using MauiApp1.BackEnd.Database;
using MauiApp1.Components;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private readonly DataContext _dbContext;
        public MainPage(DataContext dataContext)
        {
            InitializeComponent();
            _dbContext = dataContext;
        }


        private void OnOpenMenu(object? sender, EventArgs e)
        {
            this.ShowPopup(new PopupMenuPage(_dbContext));
        }
    }
}

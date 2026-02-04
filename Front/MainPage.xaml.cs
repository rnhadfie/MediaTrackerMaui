using CommunityToolkit.Maui.Extensions;
using MauiApp1.BackEnd.Database;
using MauiApp1.Components;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        DatabaseService _databaseService = new DatabaseService();
        public MainPage()
        {
            InitializeComponent();

            _databaseService.InitTables();
        }


        private void OnOpenMenu(object? sender, EventArgs e)
        {
            this.ShowPopup(new PopupMenuPage());
        }
    }
}

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.Front.Components.Books;


namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

        }

        private async void OnOpenBooksClicked(object sender, EventArgs e)
        {
            // Navigate to the Books page via the named route using absolute navigation
            await Shell.Current.GoToAsync("bookhome");
        }
    }
}

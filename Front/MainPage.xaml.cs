using CommunityToolkit.Maui.Extensions;
using MauiApp1.Components;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        private void OnOpenMenu(object? sender, EventArgs e)
        {
            this.ShowPopup(new PopupMenuPage());
        }
    }
}

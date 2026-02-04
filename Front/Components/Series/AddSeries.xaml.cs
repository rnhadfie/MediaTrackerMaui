using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;

namespace MauiApp1.Front.Components.Series;

public partial class AddSeries : ContentView
{
    SeiresSetupViewModel _viewModel;
    SeriesController controller;

    public AddSeries()
	{
		InitializeComponent();

        controller = new SeriesController();
        _viewModel = controller.GetSeriesSetup();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MainPage");
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        //Get and save values

        string title = Series_TitleTextField.Value.ToString();
        if (title.Length > 0)
        {
            //Validate 
        }
        string author = Series_AuthorTextField.Value.ToString();
        if (author.Length > 0)
        {
            //Validate 
        }

        var s = new MauiApp1.Service.Modals.Series
        {
            Title = title,
            Author = author,
            Publisher = (string)Series_publisherTextField.Value,
            Artist = (string)Series_ArtistTextField.Value,
            CollectionStatus = MauiApp1.Shared.Enums.CollectionStatus.Collecting,
            TotalVolumes = int.Parse((string)Series_VolumeTextField.Value)
        };

        await Shell.Current.GoToAsync("///MainPage");
    }
}
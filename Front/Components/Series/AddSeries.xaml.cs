
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;

namespace MauiApp1.Front.Components.Series;

public partial class AddSeries
{
    SeiresSetupViewModel _viewModel;
    SeriesController controller;
    private readonly DataContext _dbContext;
    public AddSeries(DataContext dataContext)
	{
		InitializeComponent();
        _dbContext = dataContext;

        GetSetup();
    }

    private async void GetSetup()
    {
        controller = new SeriesController(_dbContext);
        _viewModel = controller.GetSeriesSetup();

        StatusCombobox.InputList = _viewModel.ListOfSeriesStatus;
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

        string totalVolumesString = Series_VolumeTextField.Value;
        int? totalVolumes = null;
        if (totalVolumesString != null && totalVolumesString.Length != 0 )
        {
            totalVolumes = int.Parse(totalVolumesString);
        }

        var s = new MauiApp1.Service.Modals.Series
        {
            SeriesId = -1,
            Title = title,
            Author = author,
            Publisher = Series_publisherTextField.Value,
            Artist = Series_ArtistTextField.Value,
            CollectionStatus = MauiApp1.Shared.Enums.CollectionStatus.Collecting,
            TotalVolumes = totalVolumes
        };

        controller.SaveNewSeries(s);

        await Shell.Current.GoToAsync("///MainPage");
    }
}
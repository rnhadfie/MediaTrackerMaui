
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Service.Modals;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Front.Components.Series;

[QueryProperty(nameof(Add), nameof(Add))]
[QueryProperty(nameof(SeriesId), "SeriesId")]
public partial class SeriesForm
{
    SeiresSetupViewModel _viewModel;
    SeriesController controller;
    private readonly DataContext _dbContext;
    private MauiApp1.Service.Modals.Series _Series;

    public SeriesForm(DataContext dataContext)
	{
		InitializeComponent();
        _dbContext = dataContext;

        GetSetup();

        _Series = new MauiApp1.Service.Modals.Series()
        {
            SeriesId = -1,
            Title = "",
        };
    }

    private bool add;

    public bool Add
    {
        get => add;
        set
        {
            OnPropertyChanged();
            add = value;
            OnPropertyChanged2();
        }
    }

    private int seriesId;
    public int SeriesId
    {
        get => seriesId; set
        {
            OnPropertyChanged();
            seriesId = value;
            OnPropertyChanged2();
        }
    }

    private async void GetSetup()
    {
        controller = new SeriesController(_dbContext);
        _viewModel = controller.GetSeriesSetup();

        StatusCombobox.InputList = _viewModel.ListOfSeriesStatus;
    }

    private void OnPropertyChanged2()
    {
        if (!add && seriesId > 0)
        {
            _Series = controller.GetSeriesInfo(seriesId);
            if (_Series != null && _Series.SeriesId > 0)
            {
                Series_TitleTextField.Value = _Series.Title;
                Series_AuthorTextField.Value = _Series.Author;
                Series_ArtistTextField.Value = _Series.Artist;
                Series_publisherTextField.Value = _Series.Publisher;
                Series_VolumeTextField.Value = _Series.TotalVolumes.ToString();
            }
            else
            {
                _Series =new MauiApp1.Service.Modals.Series()
                {
                    SeriesId = -1,
                    Title = "",
                };
            }
        }
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

             _Series.Title = title;
             _Series.Author = author;
             _Series.Publisher = Series_publisherTextField.Value;
             _Series.Artist = Series_ArtistTextField.Value;
             _Series.CollectionStatus = MauiApp1.Shared.Enums.CollectionStatus.Collecting;
             _Series.TotalVolumes = totalVolumes;

        controller.SaveNewSeries(_Series);

        await Shell.Current.GoToAsync("///MainPage");
    }
}
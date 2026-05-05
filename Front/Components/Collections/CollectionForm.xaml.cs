
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Collections;

[QueryProperty(nameof(Add), nameof(Add))]
[QueryProperty(nameof(SeriesId), "SeriesId")]
public partial class CollectionForm
{
    SeiresSetupViewModel _viewModel;
    CollectionController controller;
    private readonly DataContext _dbContext;
    private MauiApp1.Modals.Collection _Series;

    public CollectionForm(DataContext dataContext)
	{
		InitializeComponent();
        _dbContext = dataContext;

        GetSetup();

        _Series = new MauiApp1.Modals.Collection()
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
        controller = new CollectionController(_dbContext);
        _viewModel = controller.GetSeriesSetup();

        Series_Status.AddData(_viewModel.ListOfSeriesStatus);
        Series_Type.AddData(_viewModel.ListOfMediaTypes);
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
                Series_Status.Value = (int)_Series.CollectionStatus.GetValueOrDefault();
                Series_Type.Value = (int)_Series.type;
                Series_VolumeTextField.Value = _Series.TotalVolumes.ToString();
            }
            else
            {
                _Series =new MauiApp1.Modals.Collection()
                {
                    SeriesId = -1,
                    Title = "",
                };
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
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
             _Series.CollectionStatus = (CollectionStatus)Series_Status.Value;
            _Series.type = (MediaDataType)Series_Type.Value;
            _Series.TotalVolumes = totalVolumes;

        controller.SaveNewSeries(_Series);

        await Shell.Current.GoToAsync("..");
    }
}
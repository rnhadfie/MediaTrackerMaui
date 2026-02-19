using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Components.ScrollDisplay;
using MauiApp1.Service.Modals;
using Microsoft.EntityFrameworkCore;

namespace MauiApp1.Front.Components.Series;

[QueryProperty(nameof(SeriesId), "SeriesId")]
public partial class SeriesDetailView : ContentPage
{
    SeriesController controller;
    private readonly DataContext _dbContext;
    private List<DisplayViewItem> _CollectionView;
    private int CellSize = 160;

    public SeriesDetailView(DataContext dataContext)
	{
        _dbContext = dataContext;

        InitializeComponent();
        controller = new SeriesController(_dbContext);
        _CollectionView = new List<DisplayViewItem>();
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

    private void OnPropertyChanged2()
    {
        if (seriesId > 0)
        {

            _CollectionView = controller.GetSeriesItems(seriesId);
            seriesDetailView.InputList = _CollectionView;
        }
    }

    

    private void OnGridSizeChanged(object sender, EventArgs e)
    {
        //UpdateGrid();
    }

    private void UpdateGrid()
    {

        seriesDetailView.InputList = _CollectionView;
    }

    private  async void ImageButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(SeriesForm)}?Add={true}&SeriesId={-1}");
    }
}
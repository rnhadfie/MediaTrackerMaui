using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Components.ScrollDisplay;
using MauiApp1.Shared;
using Microsoft.EntityFrameworkCore;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Front.Components.Series;


public partial class SeriesView : ContentPage
{

    private SeriesController _seriesViewController;
    private readonly DataContext _dbContext;
    private List<DisplayViewItem> _TotalSeriesList;
    private List<DisplayViewItem> _SeriesList;
    private const double CellSize = 160;
    private string _TextFilter = "";
    private MediaDataType _TypeFilter = MediaDataType.All;
    private CollectionStatus _StatusFilter = CollectionStatus.All;

    public SeriesView(DataContext dataContext)
	{
		InitializeComponent();
        _dbContext = dataContext;

        _seriesViewController = new SeriesController(dataContext);

        _TotalSeriesList = _seriesViewController.GetSeriesList();
        _TotalSeriesList.RemoveAt(0);
        _SeriesList = _TotalSeriesList;

        var _seriesTypeList = _seriesViewController.GetSeriesSetup();
        SeriesView_TypeComboBox.InputList = _seriesTypeList.ListOfMediaTypes;
        SeriesView_StatusComboBox.InputList = _seriesTypeList.ListOfSeriesStatus;

    }

    private void OnGridSizeChanged(object sender, EventArgs e)
    {
        UpdateGrid();
    }

    private void AddGridContent(int rows, int cols)
    {
        int totalList = _SeriesList.Count;
        int currentNum = 0;
        // Example: Add a Label to each cell
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (currentNum < totalList)
                {
                    DisplayViewItem viewItem = _SeriesList[currentNum++];
                    var scrollViewItem = new ScrollViewItem
                    {
                        MediaType = viewItem.Type,
                        Source = viewItem,
                        LabelText = viewItem.Name,
                    };
                    // Set the position of the label in the grid
                    Grid.SetRow(scrollViewItem, r);
                    Grid.SetColumn(scrollViewItem, c);
                    SeriesGridView.Children.Add(scrollViewItem);
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void UpdateGrid()
    {
        // Clear existing definitions to prevent duplication on size changes
        SeriesGridView.RowDefinitions.Clear();
        SeriesGridView.ColumnDefinitions.Clear();
        SeriesGridView.Children.Clear(); // Clear existing content if needed

        // Calculate the number of rows and columns based on the current size

        int numRows = (int)Math.Floor(SeriesLayout.Height / CellSize);
        int numCols = (int)Math.Floor(SeriesGridView.Width / CellSize);

        // Add RowDefinitions
        for (int i = 0; i < numRows; i++)
        {
            // Use Star (*) for proportional resizing if you prefer, or Absolute for fixed size
            SeriesGridView.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        }

        // Add ColumnDefinitions
        for (int i = 0; i < numCols; i++)
        {
            // Use Star (*) for proportional resizing if you prefer, or Absolute for fixed size
            SeriesGridView.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        AddGridContent(numRows, numCols);
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        await HideElementWithSlideAnimation(!FilterDrawer.IsVisible);
        //FilterDrawer.IsOpen = true;
    }

    private async Task HideElementWithSlideAnimation(bool show)
    {
        FilterDrawer.IsVisible = show;

        FilterDrawer.TranslationY = 0;
        FilterDrawer.Opacity = 1;
    }

    private void FilterGrid()
    {
        _SeriesList = _TotalSeriesList.FindAll(x =>
        {
            bool textMatch = string.IsNullOrEmpty(_TextFilter) || x.Name.Contains(_TextFilter, StringComparison.OrdinalIgnoreCase);
            bool typeMatch = x.Type == MediaDataType.All || x.Type == MediaDataType.Series || x.Type == _TypeFilter;
            //bool statusMatch = _StatusFilter == CollectionStatus.All || x. == _StatusFilter;
            return textMatch && typeMatch;
        });
    }

    private async void FilterButton_Clicked(object sender, EventArgs e)
    {
        _TextFilter = SeriesView_Search.Value;
        var typeValue = SeriesView_TypeComboBox.Value;

        var type = (typeValue != null) ? (TextValuePair<int>)SeriesView_TypeComboBox.Value : null;
        var status = (TextValuePair<int>)SeriesView_StatusComboBox.Value;

        _TypeFilter = type != null ? (MediaDataType)type.Value : MediaDataType.All;
        _StatusFilter = status != null ? (CollectionStatus)status.Value : CollectionStatus.All;
        await HideElementWithSlideAnimation(false);

        FilterGrid();

        UpdateGrid();
    }
}
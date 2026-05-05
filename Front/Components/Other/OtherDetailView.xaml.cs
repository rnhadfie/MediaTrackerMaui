using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Other;

public partial class OtherDetailView : ContentPage
{

    private OtherController _OtherController;
    private readonly DataContext _dbContext;
    private List<DisplayViewItem> _OtherList;
    private List<BackEnd.Modals.Other> _AllItems;
    private bool _TypeLoaded = false;
    private bool _FormatLoaded = false;

    private string _TextFilter;


    public OtherDetailView(DataContext dataContext)
	{
		_dbContext = dataContext;
        _OtherController = new OtherController(_dbContext);
        InitializeComponent();
        BindingContext = this;

        var setup = _OtherController.GetOtherSetup();

        OnLoad();

    }


    private void OnLoad()
    {
        if (_TypeLoaded && _FormatLoaded)
        {
            return;
        }
        BookFilter filter = new BookFilter() { Type = -1 };
            _AllItems = _OtherController.GetAllOtehrItems();
            _OtherList = GetDisplayList();
            UpdateGrid();
        
    }

    private void OnGridSizeChanged(object sender, EventArgs e)
    {
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        OtherGridView.InputList = _OtherList;
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        await HideElementWithSlideAnimation(!Other_FilterDrawer.IsVisible);
    }

    private async Task HideElementWithSlideAnimation(bool show)
    {
        Other_FilterDrawer.IsVisible = show;

        Other_FilterDrawer.TranslationY = 0;
        Other_FilterDrawer.Opacity = 1;
    }

    private async void FilterButton_Clicked(object sender, EventArgs e)
    {
        _TextFilter = OtherDetailView_Search.Value;

        await HideElementWithSlideAnimation(false);

        _OtherList = GetDisplayList();

        UpdateGrid();
    }

    private List<DisplayViewItem> GetDisplayList()
    {


        var list = _AllItems.FindAll(x =>
        {
            bool textMatch = string.IsNullOrEmpty(_TextFilter);
            return textMatch;
        });

        return list.Select(x => new DisplayViewItem
        {
            Id = x.Id,
            Name = x.Name,
            Type = MediaDataType.Other,
            Cover = x.Image
        }).ToList();
    }
}
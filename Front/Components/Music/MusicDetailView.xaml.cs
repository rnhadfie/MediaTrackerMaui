
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Modals;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Music;

public partial class MusicDetailView : ContentPage
{

    private MusicController _MusicController;
    private readonly DataContext _dbContext;
    private List<DisplayViewItem> _CdList;
    private List<Cd> _AllCds;
    private bool _TypeLoaded = false;
    private bool _FormatLoaded = false;

    private string _TextFilter;
    //private BookType? _TypeFilter = null;
    private BookFormat? _FormatFilter = null;
    private Genre? _GenreFilter = null;


    public MusicDetailView(DataContext dataContext)
	{
		_dbContext = dataContext;
        _MusicController = new MusicController(_dbContext);
        InitializeComponent();
        BindingContext = this;

        var setup = _MusicController.GetMusicSetup();

        MusicDetailView_Langauge.InputList = setup.Langauge;
        MusicDetailView_Genre.InputList = setup.Genres;
        UpdateGrid();
    }

    

    private void OnGridSizeChanged(object sender, EventArgs e)
    {
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        MusicGridView.InputList = _CdList;
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        await HideElementWithSlideAnimation(!Cd_FilterDrawer.IsVisible);
    }

    private async Task HideElementWithSlideAnimation(bool show)
    {
        Cd_FilterDrawer.IsVisible = show;

        Cd_FilterDrawer.TranslationY = 0;
        Cd_FilterDrawer.Opacity = 1;
    }

    private async void FilterButton_Clicked(object sender, EventArgs e)
    {
        _TextFilter = MusicDetailView_Search.Value;
        var formatValue = MusicDetailView_Langauge.Value;
        var genreValue = MusicDetailView_Genre.Value;

        var format = formatValue != null ? (TextValuePair<int>)formatValue : null;
        var genre = genreValue != null ? (TextValuePair<int>)genreValue : null;

        _FormatFilter = format != null ? (BookFormat)format.Value   : null;
         _GenreFilter = genre != null ? (Genre)genre.Value : null;
        await HideElementWithSlideAnimation(false);

        _CdList = GetDisplayList();

        UpdateGrid();
    }
    
    private List<DisplayViewItem> GetDisplayList()
    {


        var list = _AllCds.FindAll(x =>
        {
            bool textMatch = string.IsNullOrEmpty(_TextFilter) 
                            || (x.Name.Contains(_TextFilter, StringComparison.OrdinalIgnoreCase)
                             || (x.Artist ?? "").Contains(_TextFilter, StringComparison.OrdinalIgnoreCase));

            bool formatMatch = _FormatFilter == null || x.Language == (int)_FormatFilter.Value;
            bool genreMatch = _GenreFilter == null || x.MusicGenre == (int)_GenreFilter.Value;
            return textMatch && formatMatch && genreMatch;
        });

        return list.Select(x => new DisplayViewItem
        {
            Id = x.Id,
            Name = x.Name,
            Type = MediaDataType.Cd,
            Cover = x.Cover
        }).ToList();
    }
}
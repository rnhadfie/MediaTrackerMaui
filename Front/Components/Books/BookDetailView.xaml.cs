using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.Controllers;
using MauiApp1.Modals;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Books;

[QueryProperty(nameof(BookFormat), nameof(BookFormat))]
[QueryProperty(nameof(BookType), nameof(BookType))]
public partial class BookDetailView : ContentPage
{

    private BookController _BookController;
    private readonly DataContext _dbContext;
    private List<DisplayViewItem> _BookList;
    private List<Book> _AllBooks;
    private bool _TypeLoaded = false;
    private bool _FormatLoaded = false;

    private string _TextFilter;
    private BookType? _TypeFilter = null;
    private BookFormat? _FormatFilter = null;
    private Genre? _GenreFilter = null;


    public BookDetailView(DataContext dataContext)
	{
		_dbContext = dataContext;
        _BookController = new BookController(_dbContext);
        InitializeComponent();
        BindingContext = this;

        var setup = _BookController.GetBookSetup();

        BookDetailView_Format.InputList = setup.Format;
        BookDetailView_Genre.InputList = setup.Genre;
        BookDetailView_TypeComboBox.InputList = setup.Type;
    }

    private int _BookFormat;

    public int BookFormat
    {
        get => _BookFormat;
        set
        {
            OnPropertyChanged();
            _BookFormat = value;
            OnLoad();
            _FormatLoaded = true;
        }
    }

    private int _BookType;
    public int BookType
    {
        get => _BookType; set
        {
            OnPropertyChanged();
            _BookType = value;
            OnLoad();
            _TypeLoaded = true;
        }
    }

    private void OnLoad()
    {
        if (_TypeLoaded && _FormatLoaded)
        {
            return;
        }
        BookFilter filter = new BookFilter() { Type = -1 };
        if (_BookType > 0 )
        {
            filter.Type = _BookType;
        }
            
       
        if (_BookFormat > 0)
        {
            filter.Format = _BookFormat;
        }

        if ((_TypeLoaded && !_FormatLoaded) || (!_TypeLoaded && _FormatLoaded))
        {

            _AllBooks = _BookController.GetAllBooks(filter);
            _BookList = GetDisplayList();
            UpdateGrid();
        }

    // Loading overlay
    Grid _loadingOverlay;
    ActivityIndicator _loadingIndicator;

    void EnsureLoadingOverlay()
    {
        if (_loadingOverlay != null)
            return;

        var original = Content as View;
        var root = new Grid();
        if (original != null)
            root.Children.Add(original);

        var overlay = new Grid
        {
            BackgroundColor = Colors.Black.WithAlpha(0.4f),
            IsVisible = false,
            InputTransparent = false
        };

        var indicator = new ActivityIndicator
        {
            IsRunning = true,
            IsVisible = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Color = Colors.White
        };

        overlay.Children.Add(indicator);
        root.Children.Add(overlay);

        Content = root;

        _loadingOverlay = overlay;
        _loadingIndicator = indicator;
    }

    void ShowLoading()
    {
        EnsureLoadingOverlay();
        _loadingOverlay.IsVisible = true;
        _loadingIndicator.IsRunning = true;
    }

    void HideLoading()
    {
        if (_loadingOverlay == null) return;
        _loadingOverlay.IsVisible = false;
        _loadingIndicator.IsRunning = false;
    }
    }

    private void OnGridSizeChanged(object sender, EventArgs e)
    {
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        BookGridView.InputList = _BookList;
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        await HideElementWithSlideAnimation(!FilterDrawer.IsVisible);
    }

    private async Task HideElementWithSlideAnimation(bool show)
    {
        FilterDrawer.IsVisible = show;

        FilterDrawer.TranslationY = 0;
        FilterDrawer.Opacity = 1;
    }

    private async void FilterButton_Clicked(object sender, EventArgs e)
    {
        _TextFilter = BookDetailView_Search.Value;
        var typeValue = BookDetailView_TypeComboBox.Value;
        var formatValue = BookDetailView_Format.Value;
        var genreValue = BookDetailView_Genre.Value;

        var type = (typeValue != null) ? (TextValuePair<int>)typeValue : null;
        var format = formatValue != null ? (TextValuePair<int>)formatValue : null;
        var genre = genreValue != null ? (TextValuePair<int>)genreValue : null;

         _TypeFilter = type != null ? (BookType)type.Value : null;
        _FormatFilter = format != null ? (BookFormat)format.Value   : null;
         _GenreFilter = genre != null ? (Genre)genre.Value : null;
        await HideElementWithSlideAnimation(false);

        _BookList = GetDisplayList();

        UpdateGrid();
    }

    private List<DisplayViewItem> GetDisplayList()
    {


        var list = _AllBooks.FindAll(x =>
        {
            bool textMatch = string.IsNullOrEmpty(_TextFilter) 
                            || (x.Title.Contains(_TextFilter, StringComparison.OrdinalIgnoreCase)
                            || (x.Author ?? "").Contains(_TextFilter, StringComparison.OrdinalIgnoreCase)
                             || (x.Artist ?? "").Contains(_TextFilter, StringComparison.OrdinalIgnoreCase)
                             || (x.Publisher ?? "").Contains(_TextFilter, StringComparison.OrdinalIgnoreCase));

            bool typeMatch = _TypeFilter == null ||  x.Type == (int)_TypeFilter.Value;
            bool formatMatch = _FormatFilter == null || x.Format == (int)_FormatFilter.Value;
            bool genreMatch = _GenreFilter == null || x.Format == (int)_GenreFilter.Value;
            return textMatch && formatMatch && genreMatch && typeMatch;
        });

        return list.Select(x => new DisplayViewItem
        {
            Id = x.Id,
            Name = x.Title,
            Type = MediaDataType.Book,
            Cover = x.Cover
        }).ToList();
    }
}
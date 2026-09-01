

using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Controllers.ViewModels;
using Microcharts;

using SkiaSharp;
using System.Collections.Specialized;
using System.ComponentModel;
using static MauiApp1.BackEnd.Shared.Enums;
using System.Collections.ObjectModel;

namespace MauiApp1.Front.Components.Books;

public partial class BookVisualization : ContentView
{
    private BookSetupViewModel? _subscribedItemsSource;
    private INotifyCollectionChanged? _subscribedBooksCollection;
    private CancellationTokenSource? _refreshCts;
    private const int RefreshDebounceMs = 250;

    public static readonly BindableProperty ItemsSourceProperty =
     BindableProperty.Create(
         nameof(ItemsSource),
         typeof(BookSetupViewModel),
         typeof(BookVisualization),
         new BookSetupViewModel(), // default must be of the correct type
         BindingMode.TwoWay,
         propertyChanged: OnItemsSourceChanged);

    public BookSetupViewModel ItemsSource
    {
        get => (BookSetupViewModel)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(
            nameof(SelectedItem),
            typeof(TextValuePair<string,int>),
             typeof(BookVisualization),
            new TextValuePair<string, int>("All", 0),
            BindingMode.TwoWay,
            propertyChanged: OnDropDownChanged);

    public TextValuePair<string, int> SelectedItem
    {
        get => (TextValuePair<string, int>)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    #region Display Properties
    
    public static readonly BindableProperty publisherListProperty =
        BindableProperty.Create(
            nameof(PublisherListData),
            typeof(List<TextValuePair<string, int>>),
             typeof(BookVisualization),
            new List<TextValuePair<string, int>>(),
            BindingMode.TwoWay);

    protected List<TextValuePair<string, int>> PublisherListData
    {
        get => (List<TextValuePair<string, int>>)GetValue(publisherListProperty);
        set => SetValue(publisherListProperty, value);
    }

    public static readonly BindableProperty readPecentageProperty =
        BindableProperty.Create(
            nameof(ReadPecentage),
            typeof(string),
             typeof(BookVisualization),
            string.Empty,
            BindingMode.TwoWay);

    protected string ReadPecentage
    {
        get => (string)GetValue(readPecentageProperty);
        set => SetValue(readPecentageProperty, value);
    }

    public static readonly BindableProperty totalSeriesProperty =
        BindableProperty.Create(
            nameof(TotalSeries),
            typeof(string),
             typeof(BookVisualization),
            string.Empty,
            BindingMode.TwoWay);

    protected string TotalSeries
    {
        get => (string)GetValue(totalSeriesProperty);
        set => SetValue(totalSeriesProperty, value);
    }

    public static readonly BindableProperty ongoingSeriesProperty =
        BindableProperty.Create(
            nameof(OngoingSeries),
            typeof(string),
             typeof(BookVisualization),
            string.Empty,
            BindingMode.TwoWay);

    protected string OngoingSeries
    {
        get => (string)GetValue(ongoingSeriesProperty);
        set => SetValue(ongoingSeriesProperty, value);
    }

    public static readonly BindableProperty bookTypeChartDataProperty =
     BindableProperty.Create(
         nameof(BookTypeChartData),
         typeof(List<TextValuePair<string, int>>),
          typeof(BookVisualization),
         new List<TextValuePair<string, int>>(),
         BindingMode.TwoWay);

    protected List<TextValuePair<string, int>> BookTypeChartData
    {
        get => (List<TextValuePair<string, int>>)GetValue(bookTypeChartDataProperty);
        set => SetValue(bookTypeChartDataProperty, value);
    }


    public static readonly BindableProperty genreChartDataProperty =
    BindableProperty.Create(
        nameof(GenreChartData),
        typeof(List<TextValuePair<string, int>>),
         typeof(BookVisualization),
        new List<TextValuePair<string, int>>(),
        BindingMode.TwoWay);

    protected List<TextValuePair<string, int>> GenreChartData
    {
        get => (List<TextValuePair<string, int>>)GetValue(genreChartDataProperty);
        set => SetValue(genreChartDataProperty, value);
    }

    public static readonly BindableProperty collectionStatusChartDataProperty =
    BindableProperty.Create(
        nameof(CollectionStatusChartData),
        typeof(Chart),
         typeof(BookVisualization),
        null,
        BindingMode.TwoWay);

    protected Chart CollectionStatusChartData
    {
        get => (Chart)GetValue(collectionStatusChartDataProperty);
        set => SetValue(collectionStatusChartDataProperty, value);
    }

    public static readonly BindableProperty bookFormatChartDataProperty =
BindableProperty.Create(
    nameof(BookFormatChartData),
    typeof(Chart),
     typeof(BookVisualization),
    null,
    BindingMode.TwoWay);

    protected Chart BookFormatChartData
    {
        get => (Chart)GetValue(bookFormatChartDataProperty);
        set => SetValue(bookFormatChartDataProperty, value);
    }



    #endregion



    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BookVisualization view)
        {
            // Avoid reassigning the BindableProperty here (would re-trigger the callback).
            var oldVm = oldValue as BookSetupViewModel;
            var newVm = newValue as BookSetupViewModel;

            // Unsubscribe from previous VM events
            if (oldVm != null)
            {
                if (oldVm is INotifyPropertyChanged oldInpc)
                    oldInpc.PropertyChanged -= view.OnItemsSourcePropertyChanged;

                if (oldVm.Books is INotifyCollectionChanged oldBooks)
                    oldBooks.CollectionChanged -= view.OnBooksCollectionChanged;
            }

            // Subscribe to new VM events so internal changes update visualizations
            if (newVm != null)
            {
                if (newVm is INotifyPropertyChanged newInpc)
                    newInpc.PropertyChanged += view.OnItemsSourcePropertyChanged;

                if (newVm.Books is INotifyCollectionChanged newBooks)
                    newBooks.CollectionChanged += view.OnBooksCollectionChanged;

                view._subscribedItemsSource = newVm;
            }

            // Debounced refresh using the current VM instance
            view.DebounceRefresh();
        }
    }

    private static void OnDropDownChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BookVisualization view)
        {
            // Do not reassign the BindableProperty here (would re-trigger the callback).
            // newValue is the updated SelectedItem; schedule a debounced refresh.
            view.DebounceRefresh();
        }
    }



    public BookVisualization()
    {
        InitializeComponent();
    }

    #region METHODS

    private void RefreshVisualizations(BookVisualization view)
    {
        view.PublisherList.ItemsSource = GetPublisherData();
        view.PublisherListData = GetPublisherData();

           var currentBooks = GetCurrentList();

            float readPercent = ItemsSource != null && currentBooks != null && currentBooks.Count > 0
                ? (float)currentBooks.Where(x => x.Read).Count() / currentBooks.Count * 100
                : 0;
        view.ReadPecentage = "Read " + readPercent.ToString() + "%";
        view.ReadText.Text = view.ReadPecentage;
        
        var currentSeries = ItemsSource != null && currentBooks != null ? ItemsSource.BookSeries.Where(x => currentBooks.Any(y => y.BookSeries == x.Id)) : new List<BookSeries>();
         view.TotalSeries = "Total Series " + (currentSeries != null ? currentSeries.Count() : 0).ToString();
        view.TotalText.Text = view.TotalSeries;


        view.OngoingSeries = "Ongoing Series " + (currentSeries != null ? currentSeries.Where(x => x.UpToDateComplete).Count() : 0).ToString();
        view.OngoingText.Text = view.OngoingSeries;

        view.BookFormatChartData = new PieChart { Entries = GetBookFormatData(), LabelTextSize = 12f };
        view.BookFormatChart.Chart = BookFormatChartData;



        view.BookTypeChartData = GetBookTypeChartData();
        view.BookTypeList.ItemsSource = BookTypeChartData;

        view.GenreChartData = GetGenreData();
        view.GenreList.ItemsSource = GenreChartData;

        view.CollectionStatusChartData = new PieChart { Entries = GetCollectionStatusData(), LabelTextSize = 12f };
        view.BooksStatusChart.Chart = BookFormatChartData;

    }

    private List<TextValuePair<string, int>> GetPublisherData()
    {
        if (ItemsSource == null || ItemsSource.Publisher == null)
            return new List<TextValuePair<string, int>>();

        var publishers = ItemsSource.Publisher;
        List<TextValuePair<string, int>> textValuePairs = new List<TextValuePair<string, int>>();

        foreach (var publisher in publishers)
        {
            var count = CountBooksForPublisher(publisher.Id);
            if (count > 0)
            {
                var tet = new TextValuePair<string, int>(publisher.PublisherName, count);
                textValuePairs.Add(tet);
            }
        }

        return textValuePairs;
    }

    private ChartEntry[] GetBookFormatData()
    {
        if (ItemsSource == null || ItemsSource.Format == null)
            return Array.Empty<ChartEntry>();

        var formats = ItemsSource.Format;
        List<ChartEntry> chartEntries = new List<ChartEntry>();

        foreach (var format in formats)
        {
            var books = GetCurrentList();
            if (books == null)
                continue;

            int count = books.Where(x => x.Format == (BookFormat)format.Value).Count();
            var tet = new ChartEntry(count)
            {
                Label = format.Text,
                ValueLabel = count.ToString(),
                Color = SKColor.Parse(ColourHelper.GenerateRandomColour())
            };
            chartEntries.Add(tet);
        }

        return chartEntries.ToArray();
    }

    private List<TextValuePair<string, int>> GetBookTypeChartData()
    {
        if (ItemsSource == null || ItemsSource.Type == null)
            return new List<TextValuePair<string, int>>();

        var bookTypes = ItemsSource.Type;
        List<TextValuePair<string, int>> typeData = new List<TextValuePair<string, int>>();

        foreach (var bookType in bookTypes)
        {
            var books = GetCurrentList();
            if (books == null) return typeData;
            if (bookType == null) { continue; }

            int count = books.Where(x => x.Type != null && (int)x.Type == bookType.Value).Count();
            var tet = new TextValuePair<string, int>(bookType.Text, count);
            typeData.Add(tet);
        }

        return typeData;
    }

    private List<TextValuePair<string, int>> GetGenreData()
    {
        if (ItemsSource == null || ItemsSource.Genre == null)
            return new List<TextValuePair<string, int>>();
        var genres = ItemsSource.Genre;
        List<TextValuePair<string, int>> textValuePairs = new List<TextValuePair<string, int>>();
        foreach (var genre in genres)
        {
            var books = GetCurrentList();
            if (books == null)
            {
                return textValuePairs;
            }
            int count = books.Where(x => BookMatchesGenre(x, genre.Value)).Count();
            if (count > 0)
            {
                var tet = new TextValuePair<string, int>(genre.Text, count);
                textValuePairs.Add(tet);
            }
        }
        return textValuePairs;
    }
    private ChartEntry[] GetCollectionStatusData()
    {
        if (ItemsSource == null)
            return Array.Empty<ChartEntry>();
        var books = GetCurrentList();
        if (books == null) return Array.Empty<ChartEntry>();

        var currentSeries = ItemsSource != null && books != null ? ItemsSource.BookSeries.Where<BookSeries>(x => books.Any(y => y.BookSeries == x.Id)) : [];

        int collectingCount = currentSeries.Where(x => x.Collecting == "Y").Count();
        int collectedCount = currentSeries.Where(x => x.Collecting == "C").Count();
        int notCollectingCount = currentSeries.Where(x => x.Collecting == "N").Count();
        var collectedEntry = new ChartEntry(collectingCount)
        {
            Label = CollectionStatusText("C"),
            ValueLabel = collectedCount.ToString(),
            Color = SKColor.Parse(ColourHelper.GenerateRandomColour())
        };

        var collectingEntry = new ChartEntry(collectingCount)
        {
            Label = CollectionStatusText("Y"),
            ValueLabel = collectingCount.ToString(),
            Color = SKColor.Parse(ColourHelper.GenerateRandomColour())
        };
        var notCollectingEntry = new ChartEntry(notCollectingCount)
        {
            Label = CollectionStatusText("N"),
            ValueLabel = notCollectingCount.ToString(),
            Color = SKColor.Parse(ColourHelper.GenerateRandomColour())
        };
        return new[] { collectedEntry, collectingEntry, notCollectingEntry };
    }

    private int CountBooksForPublisher(int publisherId)
    {
        var books = GetCurrentList();
        if (books == null) return 0;

        int count = books.Where(x => x.Publisher == publisherId).Count();
        return count;
    }

    private bool BookMatchesGenre(BookDT book, int genreId)
    {
        if (book.Genre == null)
            return false;
        List<int> genreList = book.Genre;
        return genreList.Contains(genreId);
    }


    private ObservableCollection<BookDT> GetCurrentList()
    {
        if (SelectedItem == null)
        {
            return ItemsSource == null ? new ObservableCollection<BookDT>() : ItemsSource.Books;
        }
        if (ItemsSource == null)
        { 
            return new ObservableCollection<BookDT>();
        }
        switch (SelectedItem.Value)
        {
            case (int)BookType.Novel:
                return new ObservableCollection<BookDT>(ItemsSource.Books.Where(x => x.Type == BookType.Novel));
            case (int)BookType.GraphicNovel:
                return new ObservableCollection<BookDT>(ItemsSource.Books.Where(x => x.Type == BookType.GraphicNovel));
            case (int)BookType.Manga:
                return new ObservableCollection<BookDT>(ItemsSource.Books.Where(x => x.Type == BookType.Manga));
            case (int)BookType.LightNovel:
                return new ObservableCollection<BookDT>(ItemsSource.Books.Where(x => x.Type == BookType.LightNovel));
            case (int)BookType.ArtBook:
                return new ObservableCollection<BookDT>(ItemsSource.Books.Where(x => x.Type == BookType.ArtBook));
            default:
                return new ObservableCollection<BookDT>(ItemsSource.Books);
        }
    }

    private void OnItemsSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Debounced refresh when any property on the view model changes
        DebounceRefresh();
    }

    private void OnBooksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Debounced refresh when the underlying Books collection changes
        DebounceRefresh();
    }

    private void DebounceRefresh()
    {
        // Cancel previous pending refresh
        _refreshCts?.Cancel();
        _refreshCts?.Dispose();
        _refreshCts = new CancellationTokenSource();
        var token = _refreshCts.Token;

        // Run the delayed refresh task
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(RefreshDebounceMs, token).ConfigureAwait(false);
                if (token.IsCancellationRequested) return;
                MainThread.BeginInvokeOnMainThread(() => RefreshVisualizations(this));
            }
            catch (OperationCanceledException)
            {
                // expected when cancelled
            }
        }, token);
    }

    private string CollectionStatusText(string id)
    {
        switch (id)
        {
            case "C":
                return "Collection";
            case "N":
                return "No Collecting";
            case "Y":
                return "Collecting";
            default:
                return "Unknown";
        }
    }

    #endregion

}

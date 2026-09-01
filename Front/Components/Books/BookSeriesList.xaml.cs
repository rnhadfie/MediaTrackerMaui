using MauiApp1.BackEnd.Models;
using MauiApp1.Controllers;
using System.Collections.ObjectModel;

namespace MauiApp1.Front.Components.Books;

public partial class BookSeriesList : ContentView
{
    private BookController _book;
    public BookSeriesList()
    {
        InitializeComponent();
        _book = new BookController();
    }

    private bool filtering = false;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable<BookSeries>),
        typeof(BookSeriesList),
        default(IEnumerable<BookSeries>),
        propertyChanged: OnItemsSourceChanged);

    public IEnumerable<BookSeries> ItemsSource
    {
        get => (IEnumerable<BookSeries>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IEnumerable<BookSeries> OriginalBookSeries { get; set; }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BookSeriesList control)
        {
            control.OnItemsSourceChanged((IEnumerable<BookSeries>)oldValue, (IEnumerable<BookSeries>)newValue);
        }
    }

    private void OnItemsSourceChanged(IEnumerable<BookSeries> oldValue, IEnumerable<BookSeries> newValue)
    {
        if (newValue == null)
        {
            BookSeriesListDataGrid.ItemsSource = null;
            return;
        }

        BookSeriesListDataGrid.ItemsSource = (ObservableCollection<BookSeries>)newValue;
        if (!filtering)
        {
            OriginalBookSeries = (ObservableCollection<BookSeries>)newValue;
        }
    }

    public static readonly BindableProperty EmptyViewTextProperty =
        BindableProperty.Create(
            nameof(EmptyViewText),
            typeof(string),
            typeof(BookSeriesList),
            default(string),
            BindingMode.OneWay);

    public string EmptyViewText
    {
        get => (string)GetValue(EmptyViewTextProperty);
        set => SetValue(EmptyViewTextProperty, value);
    }

    public static readonly BindableProperty SearchTextProperty =
        BindableProperty.Create(
            nameof(SearchText),
            typeof(string),
            typeof(BookSeriesList),
            "",
            BindingMode.TwoWay);

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    private void TextField_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            BookSeriesListDataGrid.ItemsSource = ItemsSource.Where(x => x.Title.Contains(SearchText) || x.Author.Contains(SearchText)).ToList();
        }
    }

    private void TextField_TextChanged(object sender, TextChangedEventArgs e)
    {
        filtering = !string.IsNullOrWhiteSpace(e.NewTextValue);
        if (e.NewTextValue != null)
        {
            filtering = true;
            SearchText = e.NewTextValue;
            List<BookSeries> fitleredList = (List<BookSeries>)OriginalBookSeries;
            BookSeriesListDataGrid.ItemsSource = fitleredList.FindAll(x => x.Title.ToLower().Contains(SearchText.ToLower()) || (!string.IsNullOrWhiteSpace(x.Author) && x.Author.ToLower().Contains(SearchText.ToLower()))).ToList();
        }

    }

    public async Task<bool> LoadSeriesAsync()
    {
        OriginalBookSeries = await _book.GetAllBookSeriesAsync();
        ItemsSource = OriginalBookSeries;
        BookSeriesListDataGrid.ItemsSource = (System.Collections.IList)OriginalBookSeries;
        return true;
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("/BookSeriesForm?edit=true");
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn)
            {
                BookSeries bookSeries = (BookSeries)btn.CommandParameter;

                var action = await Application.Current.MainPage.DisplayActionSheet("Actions", "Cancel", null, "View", "Edit", "Delete");

                if (action == "Cancel" || string.IsNullOrEmpty(action))
                    return;

                switch (action)
                {
                    case "View":
                        await Shell.Current.GoToAsync($"/BookSeriesForm?id={bookSeries.Id}&edit=false");
                        break;
                    case "Edit":
                        await Shell.Current.GoToAsync($"/BookSeriesForm?id={bookSeries.Id}&edit=true");
                        break;
                    case "Delete":
                        var confirm = await Application.Current.MainPage.DisplayAlert("Confirm", "Delete this item?", "Yes", "No");
                        if (confirm)
                        {
                            var result = await _book.DeleteBookSeriesAsync(bookSeries);
                            if (result > 0)
                            {
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Item deleted: {bookSeries.Title}", "OK");
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Failed to delete: {bookSeries.Title}", "OK");
                            }
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

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
        typeof(IEnumerable<BookItem>),
        typeof(BookSeriesList),
        default(IEnumerable<BookItem>),
        propertyChanged: OnItemsSourceChanged);

    public IEnumerable<BookItem> ItemsSource
    {
        get => (IEnumerable<BookItem>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IEnumerable<BookItem> OriginalBookSeries { get; set; }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BookSeriesList control)
        {
            control.OnItemsSourceChanged((IEnumerable<BookItem>)oldValue, (IEnumerable<BookItem>)newValue);
        }
    }

    private void OnItemsSourceChanged(IEnumerable<BookItem> oldValue, IEnumerable<BookItem> newValue)
    {
        if (newValue == null)
        {
            BookSeriesListDataGrid.ItemsSource = null;
            return;
        }

        BookSeriesListDataGrid.ItemsSource = (ObservableCollection<BookItem>)newValue;
        if (!filtering)
        {
            OriginalBookSeries = (ObservableCollection<BookItem>)newValue;
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
            //BookSeriesListDataGrid.ItemsSource = ItemsSource.Where(x => x.Title.Contains(SearchText) || x.Author.Contains(SearchText)).ToList();
        }
    }

    private void TextField_TextChanged(object sender, TextChangedEventArgs e)
    {
        filtering = !string.IsNullOrWhiteSpace(e.NewTextValue);
        if (e.NewTextValue != null)
        {
            filtering = true;
            SearchText = e.NewTextValue;
            List<BookItem> fitleredList = (List<BookItem>)OriginalBookSeries;
           // BookSeriesListDataGrid.ItemsSource = fitleredList.FindAll(x => x.Title.ToLower().Contains(SearchText.ToLower()) || (!string.IsNullOrWhiteSpace(x.Author) && x.Author.ToLower().Contains(SearchText.ToLower()))).ToList();
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
        // Series/items are now managed on the Book form; open book form to add a book and its items
        await Shell.Current.GoToAsync("/BookForm?id=0&edit=true");
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn)
            {
                BookItem bookSeries = (BookItem)btn.CommandParameter;

                var action = await Application.Current.MainPage.DisplayActionSheet("Actions", "Cancel", null, "View", "Edit", "Delete");

                if (action == "Cancel" || string.IsNullOrEmpty(action))
                    return;

                switch (action)
                {
                    case "View":
                        // Open the parent book for viewing (bookSeries.Series stores the parent book id)
                        await Shell.Current.GoToAsync($"/BookForm?id={bookSeries.Series}&edit=false");
                        break;
                    case "Edit":
                        await Shell.Current.GoToAsync($"/BookForm?id={bookSeries.Series}&edit=true");
                        break;
                    case "Delete":
                        var confirm = await Application.Current.MainPage.DisplayAlert("Confirm", "Delete this item?", "Yes", "No");
                        if (confirm)
                        {
                            var result = await _book.DeleteBookSeriesAsync(bookSeries);
                            if (result > 0)
                            {
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Item deleted: {bookSeries.VolumeTitle}", "OK");
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Failed to delete: {bookSeries.VolumeTitle}", "OK");
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

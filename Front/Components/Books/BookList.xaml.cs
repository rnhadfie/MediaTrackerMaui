using MauiApp1.BackEnd.Models;
using MauiApp1.Controllers;
using System.Collections.ObjectModel;

namespace MauiApp1.Front.Components.Books;

public partial class BookList : ContentView
{
    private BookController _book;
	public BookList()
	{
		InitializeComponent();
        _book = new BookController();

    }

    private bool filtering = false;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable<Book>),
        typeof(BookList),
        default(IEnumerable<Book>),
        propertyChanged: OnItemsSourceChanged);

    public IEnumerable<Book> ItemsSource
    {
        get => (IEnumerable<Book>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IEnumerable<Book> OriginalBooks { get; set; }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BookList control)
        {
            control.OnItemsSourceChanged((IEnumerable<Book>)oldValue, (IEnumerable<Book>)newValue);
        }
    }

    private void OnItemsSourceChanged(IEnumerable<Book> oldValue, IEnumerable<Book> newValue)
    {
        // If the inner list view uses ItemsSource binding, update it directly
        if (newValue == null)
        {
            // Clear
            BookListDataGrid.ItemsSource = null;
            return;
        }

        BookListDataGrid.ItemsSource = (ObservableCollection<Book>)newValue;
        if (!filtering)
        {
            OriginalBooks = (ObservableCollection<Book>)newValue;
        }
    }

    public static readonly BindableProperty EmptyViewTextProperty =
        BindableProperty.Create(
            nameof(EmptyViewText),
            typeof(string),
            typeof(BookList),
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
            typeof(BookList),
            "",
            BindingMode.TwoWay);

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    private void TextField_PropertyChanging(object sender, PropertyChangingEventArgs e)
    {
       
        if (!string.IsNullOrWhiteSpace(SearchText)) {
            BookListDataGrid.ItemsSource = ItemsSource.Where(x => x.Title.Contains(SearchText) || x.Author.Contains(SearchText)).ToList();
        }
    }

    private void TextField_TextChanged(object sender, TextChangedEventArgs e)
    {
        filtering = !string.IsNullOrWhiteSpace(e.NewTextValue);
        if (e.NewTextValue != null)
        {
            filtering = true;
            SearchText = e.NewTextValue;
            List<Book> fitleredList = (List<Book>)OriginalBooks;
            BookListDataGrid.ItemsSource = fitleredList.FindAll(x => x.Title.ToLower().Contains(SearchText.ToLower()) || (!string.IsNullOrWhiteSpace(x.Author) && x.Author.ToLower().Contains(SearchText.ToLower()))).ToList();
        }
        
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        // Prompt the user to choose between adding an item or a series
        var choice = await Application.Current.MainPage.DisplayActionSheet("Add", "Cancel", null, "Item", "Series");
        if (string.IsNullOrEmpty(choice) || choice == "Cancel") return;
        if (choice == "Item")
        {
            await Shell.Current.GoToAsync($"/BookForm?id=0&edit=true");
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn)
            {
                Book book = (Book)btn.CommandParameter;

                // Show a bottom-sheet style action sheet using the platform ActionSheet
                // This is a lightweight bottom sheet alternative that works across MAUI
                var action = await Application.Current.MainPage.DisplayActionSheet("Actions", "Cancel", null, "View", "Edit", "Delete");

                if (action == "Cancel" || string.IsNullOrEmpty(action))
                    return;

                // Handle actions - these are placeholders for integration with navigation/commands
                switch (action)
                {
                    case "View":
                        await Shell.Current.GoToAsync($"/BookForm?id={book.Id}&edit=false");
                        break;
                    case "Edit":
                        await Shell.Current.GoToAsync($"/BookForm?id={book.Id}&edit=true");
                        break;
                    case "Delete":
                        var confirm = await Application.Current.MainPage.DisplayAlert("Confirm", "Delete this item?", "Yes", "No");
                        if (confirm)
                        {
                            var result = await _book.DeleteBookAsync(book);
                            if (result > 0)
                            {
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Item deleted: {book.Title}", "OK");
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Deleted", $"Failed to delete: {book.Title}", "OK");
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


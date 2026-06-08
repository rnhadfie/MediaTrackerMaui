using MauiApp1.BackEnd.Models;
using MauiApp1.Front.Components.Shared;
using MauiApp1.Modals;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UraniumUI.Pages;

namespace MauiApp1.Front.Components.Books;

public partial class BookList : ContentView
{
	public BookList()
	{
		InitializeComponent();
	}

    private bool filtering = false;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable<BookDT>),
        typeof(BookList),
        default(IEnumerable<BookDT>),
        propertyChanged: OnItemsSourceChanged);

    public IEnumerable<BookDT> ItemsSource
    {
        get => (IEnumerable<BookDT>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IEnumerable<BookDT> OriginalBooks { get; set; }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BookList control)
        {
            control.OnItemsSourceChanged((IEnumerable<BookDT>)oldValue, (IEnumerable<BookDT>)newValue);
        }
    }

    private void OnItemsSourceChanged(IEnumerable<BookDT> oldValue, IEnumerable<BookDT> newValue)
    {
        // If the inner list view uses ItemsSource binding, update it directly
        if (newValue == null)
        {
            // Clear
            BookListDataGrid.ItemsSource = null;
            return;
        }

        BookListDataGrid.ItemsSource = (ObservableCollection<BookDT>)newValue;
        if (!filtering)
        {
            OriginalBooks = (ObservableCollection<BookDT>)newValue;
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

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}


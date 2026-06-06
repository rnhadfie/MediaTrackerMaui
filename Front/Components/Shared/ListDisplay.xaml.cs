using MauiApp1.BackEnd.Models;

namespace MauiApp1.Front.Components.Shared;

public partial class ListDisplay : ContentView
{
	public ListDisplay()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList<TextValuePair  <string, int>>),
            typeof(ListDisplay),
            new List<TextValuePair<string, int>>(), // default must be IList
            propertyChanged: OnItemsSourceChanged);

    public IList<TextValuePair<string, int>> ItemsSource
    {
        get => (IList<TextValuePair<string, int>>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ListDisplay view)
        {
            // Update the DataGrid's ItemsSource so the grid refreshes when parent replaces the list.
            // Safely set ItemsSource; fallback to empty list to avoid null reference in UI controls
            var items = newValue as System.Collections.IList;
            if (items != null)
            {
                view.DataGrid.ItemsSource = items;
            }
            else
            {
                view.DataGrid.ItemsSource = new System.Collections.ArrayList();
            }
        }
    }

    public static readonly BindableProperty EmptyViewTextProperty =
        BindableProperty.Create(
            nameof(EmptyViewText),
            typeof(string),
            typeof(ListDisplay),
            default(string),
            BindingMode.OneWay);

    public string EmptyViewText
    {
        get => (string)GetValue(EmptyViewTextProperty);
        set => SetValue(EmptyViewTextProperty, value);
    }

    public static readonly BindableProperty TitleTextProperty =
       BindableProperty.Create(
           nameof(Title),
           typeof(string),
           typeof(ListDisplay),
           default(string),
           BindingMode.OneWay);

    public string Title
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }
}
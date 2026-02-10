using CommunityToolkit.Maui.Core.Extensions;
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Components.Books;
using MauiApp1.Front.Components.Series;
using MauiApp1.Front.Components.Video;
using MauiApp1.Service.Modals;
using MauiApp1.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Components.ScrollDisplay;

public partial class ScrollViewDisplay : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ScrollViewDisplay), "", BindingMode.TwoWay, null, OnLabelTextChanged);

    public static readonly BindableProperty MediaTypeProperty =
        BindableProperty.Create(nameof(MediaType), typeof(MediaDataType), typeof(ScrollViewDisplay), MediaDataType.All, BindingMode.TwoWay, null, OnMediaChanged);

    public static readonly BindableProperty SourceProperty =
        BindableProperty.Create(nameof(Source), typeof(List<DisplayViewItem>), typeof(ScrollViewDisplay), default(List<DisplayViewItem>), BindingMode.TwoWay, null, OnSourceChanged);
    #endregion

    #region Properties


    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public MediaDataType MediaType
    {
        get => (MediaDataType)GetValue(MediaTypeProperty);
        set => SetValue(MediaTypeProperty, value);
    }

    public List<DisplayViewItem> Source
    {
        get => (List<DisplayViewItem>)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }


    #endregion

    #region Bindable Property changed methods

    public event EventHandler<EventArgs> Clicked;


    private void Gesture_Tap(object sender, TappedEventArgs e)
    {
        Clicked.Invoke(sender, e);
    }

    private static void OnLabelTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewDisplay)bindable;
        if (control.ScrollView_Label == null) return;
        control.ScrollView_Label.Text = (string)newValue;
    }

    private static void OnMediaChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewDisplay)bindable;
        control.MediaType = (MediaDataType)newValue;
    }

    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewDisplay)bindable;
        control.Source = (List<DisplayViewItem>)newValue;

       var list = control.Source;
        if (list == null || list.Count == 0)
        {
            control.EmptyList.Text = "No items found";
            control.EmptyList.IsVisible = true;
            control.EmptyList.HeightRequest = 100;
            return;
        }

        control.EmptyList.IsVisible = false; 
        foreach (var item in list)
        {
            var scrollViewItem = new ScrollViewItem
            {
                MediaType = item.Type,
                Source = item,
                LabelText = item.Name,
            };
            control.ScrollView_Scroll.Add(scrollViewItem);
        }


    }

    public async void onMenuSelected(object sender, EventArgs e)
    {
        var type = MediaType;
        switch (type)
        {
            case MediaDataType.Series:
                break;
            case MediaDataType.Video:

                break;
            case MediaDataType.Book:
                await Shell.Current.GoToAsync($"{nameof(BookForm)}?id={1}&isAdding={false}");
                break;
            default:
                break;
        }
    }

    #endregion

    //DataContext _DbContext;

   
    public ScrollViewDisplay()
    {

        InitializeComponent();


        ImageButton button = new ImageButton();
        button.Source = "icons_next_50.png";
        button.HeightRequest = 180;
        button.Clicked += onMenuSelected;

       ScrollView_Scroll.Add(button);

    }
}
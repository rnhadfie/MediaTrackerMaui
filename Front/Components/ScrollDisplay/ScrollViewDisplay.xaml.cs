using MauiApp1.BackEnd.Modals;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Components.ScrollDisplay;

public partial class ScrollViewDisplay : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ScrollViewDisplay), "", BindingMode.TwoWay, null, OnLabelTextChanged);

    public static readonly BindableProperty LocationProperty =
        BindableProperty.Create(nameof(LocationText), typeof(string), typeof(ScrollViewDisplay), "", BindingMode.TwoWay, null, OnLocationChanged);

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

    public string LocationText
    {
        get => (string)GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
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

    private static void OnLocationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewDisplay)bindable;
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

        // No items: clear ItemsSource to let CollectionView show its EmptyView
        if (list == null || list.Count == 0)
        {
            control.ScrollView_Arrow.IsVisible = false;
            control.ItemsCollectionView.ItemsSource = null;
            return;
        }

        control.ScrollView_Arrow.IsVisible = true;

        // Assign the list directly; CollectionView will virtualize rendering.
        control.ItemsCollectionView.ItemsSource = list.Select(i => new DisplayViewItem
        {
            Id = i.Id,
            Name = i.Name,
            Cover = i.Cover,
            Type = i.Type
        }).ToList();
    }

    public async void onButtonSelected(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(LocationText))
        {
            await Shell.Current.GoToAsync(LocationText);
        }
        
    }

    private async void ItemsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0) return;

        if (e.CurrentSelection.FirstOrDefault() is DisplayViewItem selected)
        {
            // Navigate to detail page based on MediaType
            switch (selected.Type)
            {
                case MediaDataType.Book:
                    await Shell.Current.GoToAsync($"{nameof(Front.Components.Books.BookForm)}?Add=false&BookId={selected.Id}");
                    break;
                case MediaDataType.Video:
                    await Shell.Current.GoToAsync($"{nameof(MauiApp1.Front.Components.Video.VideoForm)}?Add=false&VideoId={selected.Id}");
                    break;
                default:
                    await Shell.Current.GoToAsync($"{nameof(MauiApp1.Front.Components.Collections.CollectionDetailView)}?SeriesId={selected.Id}");
                    break;
            }
        }

        // Clear selection to allow reselecting same item later
        ItemsCollectionView.SelectedItem = null;
    }

    #endregion

    //DataContext _DbContext;

   
    public ScrollViewDisplay()
    {

        InitializeComponent();
    }
}
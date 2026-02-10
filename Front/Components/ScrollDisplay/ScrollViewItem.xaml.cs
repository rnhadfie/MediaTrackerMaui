using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Components.Books;
using MauiApp1.Front.Components.Series;
using MauiApp1.Front.Components.Video;
using MauiApp1.Service.Modals;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Components.ScrollDisplay;

public partial class ScrollViewItem : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ScrollViewItem), "", BindingMode.TwoWay, null, OnLabelTextChanged);

    public static readonly BindableProperty MediaTypeProperty =
            BindableProperty.Create(nameof(MediaType), typeof(MediaDataType), typeof(ScrollViewItem), MediaDataType.All, BindingMode.TwoWay, null);

    public static readonly BindableProperty SourceProperty =
           BindableProperty.Create(nameof(Source), typeof(DisplayViewItem), typeof(ScrollViewItem), default(DisplayViewItem), BindingMode.TwoWay, null, OnSourceChanged);

    public event EventHandler SelectionChanged;
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

    public DisplayViewItem Source
    {
        get => (DisplayViewItem)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public int ItemId 
    {
        get; set;
    }


    #endregion

    #region Bindable Property changed methods

    private static void OnLabelTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewItem)bindable;
        if (control.ScrollViewItem_Label != null)
            control.ScrollViewItem_Label.Text = (string)newValue;
    }   

    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewItem)bindable;
        var displayItem = newValue as DisplayViewItem;
        if (displayItem != null && control.ScrollViewItem_Image != null)
        {
            var bytes = displayItem.Cover ?? new byte[0];
            if (bytes.Length == 0)
            {
                // Set a default image or clear the source
                control.ScrollViewItem_Image.Source = null;
                control.ScrollViewItem_ImageLabel.Text = displayItem.Name.Substring(0,2).ToUpper();
                return;
            }
            else
            {
                control.ScrollViewItem_Image.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            control.ItemId = displayItem.Id;
        }
    }

    #endregion

    private async void ContentView_Tapped(object sender, EventArgs e)
    {
        //var control = (ScrollViewItem)sender;
        switch(MediaType)
        {
            case MediaDataType.Book:
                await Shell.Current.GoToAsync($"{nameof(BookForm)}?Add={false}&BookId={Source.Id}");
                break;
             case MediaDataType.Video:
                // Navigate to video details page
                await Shell.Current.GoToAsync($"{nameof(VideoForm)}?Add={false}&VideoId={Source.Id}");
                break;
            case MediaDataType.Series:
                // Navigate to series details page
                await Shell.Current.GoToAsync($"{nameof(SeriesForm)}?Add={false}&SeriesId={Source.Id}");
                break;
            case MediaDataType.Cd:
                break;
            default:
                break;

        }
       
    }
    public ScrollViewItem()
	{
		InitializeComponent();
	}

    private void LoadImageFromStream(byte[] imageData)
    {
        if (imageData != null && imageData.Length > 0)
        {

            // Assign the source using a Func<Stream> to let MAUI manage the stream lifecycle
            ScrollViewItem_Image.Source = ImageSource.FromStream(() => new MemoryStream(imageData));
        }
    }
}
using MauiApp1.Front.Components.Books;
using MauiApp1.Front.Components.Collections;
using MauiApp1.Front.Components.Video;
using System.Collections.Concurrent;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Components.ScrollDisplay;

public partial class ScrollViewItem : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ScrollViewItem), "", BindingMode.TwoWay, null, OnLabelTextChanged);

    public static readonly BindableProperty IdProperty =
           BindableProperty.Create(nameof(ItemId), typeof(int), typeof(ScrollViewItem), -1, BindingMode.TwoWay, null, OnIdChanged);

    public static readonly BindableProperty MediaTypeProperty =
            BindableProperty.Create(nameof(MediaType), typeof(MediaDataType), typeof(ScrollViewItem), MediaDataType.All, BindingMode.TwoWay, null);


    public static readonly BindableProperty SourceProperty =
           BindableProperty.Create(nameof(Source), typeof(byte[]), typeof(ScrollViewItem), null, BindingMode.TwoWay, null, OnSourceChanged);

    public event EventHandler SelectionChanged;
    #endregion

    // Simple in-memory cache keyed by ItemId. Avoids re-decoding images when reusing items.
    private static readonly ConcurrentDictionary<int, ImageSource> _imageCache = new();

    // Cancellation token source to cancel in-flight image loads when Source changes.
    private CancellationTokenSource? _imageLoadCts;

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

    public byte[] Source
    {
        get => (byte[])GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public int ItemId 
    {
        get => (int)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }


    #endregion

    #region Bindable Property changed methods

    private static void OnLabelTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewItem)bindable;
        if (control.ScrollViewItem_Label != null)
            control.ScrollViewItem_Label.Text = (string)newValue;
        control.LabelText = (string)newValue;

        if (control.LabelText.Length >= 2)
        {
            control.ScrollViewItem_ImageLabel.Text = control.LabelText.Substring(0, 2).ToUpper();
        }
    }

    private static void OnIdChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewItem)bindable;
        control.ItemId =  (int)newValue;
    }

    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ScrollViewItem)bindable;
        var cover = newValue as byte[];
        if (cover != null && control.ScrollViewItem_Image != null)
        {
            var bytes = cover ?? Array.Empty<byte>();
            // Kick off asynchronous image loading to avoid expensive work on the UI thread.
            var loadTask = control.LoadImageAsync(bytes);
            // Attach continuation to observe/log exceptions but do not await on UI thread
            loadTask.ContinueWith(t => { /* swallow or log t.Exception */ }, TaskScheduler.Default);
        }
    }

    #endregion

    private async void ContentView_Tapped(object sender, EventArgs e)
    {
        //var control = (ScrollViewItem)sender;
        switch(MediaType)
        {
            case MediaDataType.Book:
                await Shell.Current.GoToAsync($"{nameof(BookForm)}?Add={false}&BookId={ItemId}");
                break;
             case MediaDataType.Video:
                // Navigate to video details page
                await Shell.Current.GoToAsync($"{nameof(VideoForm)}?Add={false}&VideoId={ItemId}");
                break;
            case MediaDataType.All:
            case MediaDataType.Collection:
                // Navigate to series details page
                await Shell.Current.GoToAsync($"{nameof(CollectionDetailView)}?SeriesId={ItemId}");
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

    private Task LoadImageAsync(byte[] imageData)
    {
        // Use a local cancellation token source to cancel previous loads
        _imageLoadCts?.Cancel();
        _imageLoadCts = new CancellationTokenSource();
        var ct = _imageLoadCts.Token;

        return Task.Run(async () =>
        {
            try
            {
                if (imageData == null || imageData.Length == 0)
                {
                    // No image: show letter badge on main thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ScrollViewItem_Image.Source = null;
                        if (!string.IsNullOrEmpty(LabelText) && LabelText.Length >= 2)
                        {
                            ScrollViewItem_ImageLabel.Text = LabelText.Substring(0, 2).ToUpper();
                            ScrollViewItem_ImageLabel.IsVisible = true;
                        }
                        else
                        {
                            ScrollViewItem_ImageLabel.IsVisible = true;
                        }
                    });
                    return;
                }

                // Check cache by ItemId if available
                if (ItemId >= 0 && _imageCache.TryGetValue(ItemId, out var cached))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ScrollViewItem_Image.Source = cached;
                        ScrollViewItem_ImageLabel.IsVisible = false;
                    });
                    return;
                }

                // Copy bytes to avoid referencing the original array while decoding on background thread
                var buffer = new byte[imageData.Length];
                Array.Copy(imageData, buffer, imageData.Length);

                // Create ImageSource from stream factory. Creating the ImageSource is cheap; decoding happens later.
                var imageSource = ImageSource.FromStream(() => new MemoryStream(buffer));

                // Store in cache if we have a valid id
                if (ItemId >= 0)
                {
                    _imageCache[ItemId] = imageSource;
                }

                // Apply the ImageSource on the main thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ScrollViewItem_Image.Source = imageSource;
                    ScrollViewItem_ImageLabel.IsVisible = false;
                });
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception)
            {
                // On failure, clear the image to avoid holding corrupted data.
                MainThread.BeginInvokeOnMainThread(() => ScrollViewItem_Image.Source = null);
            }
        }, ct);
    }


}
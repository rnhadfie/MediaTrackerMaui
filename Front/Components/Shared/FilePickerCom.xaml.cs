namespace MauiApp1.Front.Components.Shared;

using Microsoft.Maui.Storage;

public partial class FilePicker : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty ButtonTextProperty =
            BindableProperty.Create(nameof(ButtonText), typeof(string), typeof(FilePicker), "", BindingMode.TwoWay, null, OnButtonTextPropertyChanged);

    public static readonly BindableProperty ImagePreviewProperty =
           BindableProperty.Create(nameof(showImagePreview), typeof(bool), typeof(FilePicker), true, BindingMode.TwoWay, null, OnShowImagePreviewChanged);

    public static readonly BindableProperty FilenamePreviewProperty =
           BindableProperty.Create(nameof(showFileName), typeof(bool), typeof(FilePicker), true, BindingMode.TwoWay, null, OnShowFileNamePreviewChanged);



    #endregion

    #region Properties

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    private byte[] _imageData;
    public byte[] ImageData
    {
        get=> _imageData;
        set { 
            _imageData = value;
            LoadImageFromStream(value);
        }
    }
    public string ImageFilenameData { get; set; }

    public bool showImagePreview
    {
        get => (bool)GetValue(ImagePreviewProperty);
        set => SetValue(ImagePreviewProperty, value);
    }

    public bool showFileName
    {
        get => (bool)GetValue(FilenamePreviewProperty);
        set => SetValue(FilenamePreviewProperty, value);
    }


    #endregion

    #region Bindable Property changed methods
    private static void OnButtonTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FilePicker)bindable;
    }

    private static void OnShowImagePreviewChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FilePicker)bindable;
        if (control.previewImage != null) control.previewImage.IsVisible = (bool)newValue;
    }

    private static void OnShowFileNamePreviewChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FilePicker)bindable;
        control.fileNameLabel.IsVisible = (bool)newValue;
    }

    #endregion

    public FilePicker()
	{
		InitializeComponent();
	}

    private async void OnPickFileButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Define the file types you want to allow (optional)
            var options = new PickOptions
            {
                PickerTitle = "Please select a file",
                FileTypes = FilePickerFileType.Images // Example: only allow images
            };

            var result = await Microsoft.Maui.Storage.FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                // Process the selected file
                string fileName = result.FileName;
                string fullPath = result.FullPath;

                ImageFilenameData = fileName;
                fileNameLabel.Text = fileName;

                var stream = await result.OpenReadAsync();
                previewImage.IsVisible = true;

                ImageData = StreamToByteArray(stream);

                LoadImageFromStream(ImageData);

                Console.WriteLine($"Selected file: {fileName}");
            }
            else
            {
                Console.WriteLine("File picking cancelled by user.");
            }
        }
        catch (TaskCanceledException)
        {
            // Handle task cancellation (e.g., user cancels the picker)
            Console.WriteLine("Picking cancelled.");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Error picking file: {ex.Message}");
        }
    }

    private static byte[] StreamToByteArray(Stream sourceStream)
    {
        // MemoryStream is IDisposable, so wrap it in a using statement
        using (var memoryStream = new MemoryStream())
        {
            // Copy the entire contents of the source stream to the memory stream
            sourceStream.CopyTo(memoryStream);

            // Return the byte array representation of the memory stream contents
            return memoryStream.ToArray();
        }
    }

    private void LoadImageFromStream(byte[] imageData)
    {
        if (imageData != null && imageData.Length > 0)
        {
            
            // Assign the source using a Func<Stream> to let MAUI manage the stream lifecycle
            previewImage.Source = ImageSource.FromStream(() => new MemoryStream(imageData));
        }
    }


}
namespace MauiApp1.Front.Components.Shared;

using Microsoft.Maui.Storage;

public partial class FilePicker : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty ButtonTextProperty =
            BindableProperty.Create(nameof(ButtonText), typeof(string), typeof(FilePicker), "", BindingMode.TwoWay, null, OnButtonTextPropertyChanged);

    /*
    public static readonly BindableProperty ImageDataProperty =
            BindableProperty.Create(nameof(ImageData), typeof(byte[]), typeof(FilePicker), [], BindingMode.TwoWay, null);

    public static readonly BindableProperty ImageFilenameDataProperty =
           BindableProperty.Create(nameof(ImageFilenameData), typeof(string), typeof(FilePicker), "", BindingMode.TwoWay, null);
    */
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

    public byte[] ImageData { get; set; }
    public string ImageFilenameData { get; set; }
    /*
    public byte[] ImageData
    {
        get => GetValue((byte[])ImageDataProperty);
        set => SetValue((byte[])ImageDataProperty, value);
    }

    public string ImageFilenameData
    {
        get => GetValue((string)ImageFilenameDataProperty);
        set => SetValue((string)ImageFilenameDataProperty, value);
    }*/

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
        control.filePickButton.Text = newValue.ToString();
    }

    private static void OnShowImagePreviewChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FilePicker)bindable;
        control.previewImage.IsVisible = (bool)newValue;
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

                // To read the file content, use OpenReadAsync
                using var stream = await result.OpenReadAsync();
                // ... use the stream to read/process the file ...

                previewImage.Source = ImageSource.FromStream(() => stream);
                previewImage.IsVisible = true;

                ImageData = new byte[stream.Length];
                stream.Read(ImageData, 0, (int)stream.Length);

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
}
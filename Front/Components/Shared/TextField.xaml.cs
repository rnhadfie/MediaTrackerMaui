namespace MauiApp1.Front.Components.Shared;

public partial class TextField : ContentView
{
    public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(TextField), string.Empty);


    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public TextField()
	{
		InitializeComponent();
	}

    public TextField(string text, 
        int width = -1, 
        int height = -1, 
        EventHandler<TextChangedEventArgs> eventHandler = null,
        string placeholder = ""
        )
    {
        InitializeComponent();
        this.textfieldLabel.Text = text;
        PlaceholderText = text;
        if (width != -1)
        {
            this.textfieldInput.WidthRequest = width;
        }
        if (height != -1)
        {
            this.textfieldInput.HeightRequest = height;
        }
        this.textfieldInput.HorizontalOptions = LayoutOptions.Fill;
        this.textfieldInput.Placeholder = placeholder;
        if (eventHandler != null)
        {
            this.textfieldInput.TextChanged += eventHandler;
        }

    }

    private async void OnEntryFocused(object sender, FocusEventArgs e)
    {
        await AnimateLabel(true);
    }

    private async void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(textfieldInput.Text))
        {
            await AnimateLabel(false);
        }
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        // Ensure the label is animated if text is programmatically set
        if (!string.IsNullOrEmpty(e.NewTextValue) && string.IsNullOrEmpty(e.OldTextValue))
        {
            AnimateLabel(true);
        }
        else if (string.IsNullOrEmpty(e.NewTextValue) && !string.IsNullOrEmpty(e.OldTextValue))
        {
            AnimateLabel(false);
        }
    }


    private async Task AnimateLabel(bool isFocused)
    {
        uint length = 150;
        Easing easing = Easing.Linear;

        if (isFocused)
        {
            // Animate label up and scale down
            await Task.WhenAll(
                textfieldLabel.TranslateTo(0, -20, length, easing),
                 textfieldLabel.ScaleTo(0.8, length, easing)
            );
        }
        else
        {
            // Animate label down and scale up
            await Task.WhenAll(
                textfieldLabel.TranslateTo(0, 0, length, easing),
                textfieldLabel.ScaleTo(1, length, easing)
            );
        }
    }

}
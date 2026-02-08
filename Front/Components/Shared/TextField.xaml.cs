
namespace MauiApp1.Front.Components.Shared;

public partial class TextField : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(TextField),"", BindingMode.TwoWay,null, OnPlaceHolderTextChanged);

    public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(int), typeof(TextField), 16, BindingMode.TwoWay, null, OnFontSizeChanged);

    public static readonly BindableProperty TextWidthProperty =
            BindableProperty.Create(nameof(TextWidth), typeof(int), typeof(TextField), 350, BindingMode.TwoWay, null, OnTextWidthChanged);

    public static readonly BindableProperty PaddingPropertiy =
            BindableProperty.Create(nameof(FieldPadding), typeof(string), typeof(TextField), "0,8,0,8");


    public static readonly BindableProperty FieldClassIdProperty =
           BindableProperty.Create(nameof(FieldClassId), typeof(string), typeof(TextField), "", BindingMode.TwoWay, null, OnClassIdChanged);

    public static readonly BindableProperty InputFieldTypeProperty =
           BindableProperty.Create(nameof(InputFieldType), typeof(Keyboard), typeof(TextField), Keyboard.Text, BindingMode.TwoWay, null, OnInputTypeChanged);

    public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(string), typeof(TextField), "", BindingMode.TwoWay, null, OnValueChanged);

   
    #endregion

    #region Properties
    public string FieldPadding
    {
        get => (string)GetValue(PaddingPropertiy);
        set => SetValue(PaddingPropertiy, value);
    }

    public Keyboard InputFieldType
    {
        get => (Keyboard)GetValue(InputFieldTypeProperty);
        set => SetValue(InputFieldTypeProperty, value);
    }

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int FontSize
    {
        get => (int)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public int TextWidth
    {
        get => (int)GetValue(TextWidthProperty);
        set => SetValue(TextWidthProperty, value);
    }

    public string FieldClassId
    {
        get => (string)GetValue(ClassIdProperty);
        set => SetValue(ClassIdProperty, value);
    }
    #endregion

    #region Bindable Property changed methods
    private static void OnPlaceHolderTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TextField)bindable;
        control.textfieldLabel.Text = newValue.ToString();
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TextField)bindable;
        control.textfieldInput.Text = newValue.ToString();
    }

    private static void OnClassIdChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TextField)bindable;
        control.textfieldInput.ClassId = newValue.ToString() + "_label";
        control.textfieldInput.ClassId = newValue.ToString() + "_input";
    }

    private static void OnTextWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TextField)bindable;
        control.ControlGrid.WidthRequest = (int)newValue;
    }

    private static void OnFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TextField)bindable;
        control.textfieldInput.FontSize = (double)newValue;
        control.textfieldLabel.FontSize = (double)newValue;
    }

    private static void OnInputTypeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TextField)bindable;
        control.textfieldInput.Keyboard = (Keyboard)newValue;
    }

    #endregion

    public TextField()
	{
		InitializeComponent();
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

        Value = e.NewTextValue;
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
namespace MauiApp1.Front.Components.Shared;

public partial class Checkbox : ContentView
{

    #region Bindable Properties
    public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(Shared.Checkbox), "", BindingMode.TwoWay, null, OnLabelTextChanged);


    public static readonly BindableProperty FieldClassIdProperty =
           BindableProperty.Create(nameof(FieldClassId), typeof(string), typeof(Shared.Checkbox), "", BindingMode.TwoWay, null, OnClassIdChanged);

    public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(bool), typeof(Shared.Checkbox), "", BindingMode.TwoWay, null, OnValueChanged);
    #endregion

    #region Properties

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }


    public string FieldClassId
    {
        get => (string)GetValue(ClassIdProperty);
        set => SetValue(ClassIdProperty, value);
    }
    #endregion

    #region Bindable Property changed methods
    private static void OnLabelTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.Checkbox)bindable;
        control.label.Text = newValue.ToString();
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.Checkbox)bindable;
        control.checkbox_section.IsChecked = (bool)newValue;
    }

    private static void OnClassIdChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.Checkbox)bindable;
        control.label.ClassId = newValue.ToString() + "_label";
        control.checkbox_section.ClassId = newValue.ToString() + "_checkbox";
    }

    #endregion

    public Checkbox()
	{
		InitializeComponent();
    }
}
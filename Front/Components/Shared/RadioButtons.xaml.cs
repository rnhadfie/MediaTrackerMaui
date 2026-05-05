using MauiApp1.Shared;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace MauiApp1.Front.Components.Shared;

public partial class RadioButtons : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(Shared.RadioButtons), "", BindingMode.TwoWay, null, OnLabelTextChanged);

    public static readonly BindableProperty groupNameProperty =
            BindableProperty.Create(nameof(GroupName), typeof(string), typeof(Shared.RadioButtons), "", BindingMode.TwoWay, null, OnGroupNameChanged);

    public static readonly BindableProperty SourceProperty =
           BindableProperty.Create(nameof(Source), typeof(List<TextValuePair<int>>), typeof(Shared.RadioButtons), null, BindingMode.TwoWay, null, OnSourceChanged);

    public static readonly BindableProperty FieldClassIdProperty =
           BindableProperty.Create(nameof(FieldClassId), typeof(string), typeof(Shared.RadioButtons), "", BindingMode.TwoWay, null, OnClassIdChanged);
    
    public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(int), typeof(Shared.RadioButtons), -1, BindingMode.TwoWay, null, OnValueChanged);
    #endregion

    #region Properties

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public string GroupName
    {
        get => (string)GetValue(groupNameProperty);
        set => SetValue(groupNameProperty, value);
    }
    
    public int Value
    {
        get => Convert.ToInt32(GetValue(ValueProperty));
        set => SetValue(ValueProperty, value);
    }

    public List<TextValuePair<int>> Source
    {
        get => (List<TextValuePair<int>>)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
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
        var control = (Shared.RadioButtons)bindable;
        if (control.radio_label != null) control.radio_label.Text = newValue?.ToString();
    }

    private static void OnGroupNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.RadioButtons)bindable;
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.RadioButtons)bindable;
        if (control == null) return;

        // Update radio buttons to reflect the new value without re-setting the BindableProperty (avoids recursion)
        var newInt = Convert.ToInt32(newValue);
        foreach (var child in control.RadioButtonGroup.Children)
        {
            if (child is RadioButton rb)
            {
                rb.IsChecked = rb.ClassId == $"RadioButton_{newInt}";
            }
        }
    }

    private static void OnClassIdChanged(BindableObject bindable, object oldValue, object newValue)
    {
        // no-op, reserved for future use
    }

    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.RadioButtons)bindable;
        if (control == null) return;

        var list = newValue as List<TextValuePair<int>>;
        control.AddData(list);
    }

    public void AddData(List<TextValuePair<int>> Buttonsource)
    {
        // Clear existing children first
        RadioButtonGroup.Children.Clear();

        if (Buttonsource == null || Buttonsource.Count == 0) return;

        foreach (var item in Buttonsource)
        {
            // Use plain string Content so platform renderers display text consistently
            var radioButton = new RadioButton
            {
                ClassId = $"RadioButton_{item.Value}",
                Content = item.Text,
                Value = item.Value,
                GroupName = this.GroupName,
                IsChecked = this.Value == item.Value,
                Margin = new Thickness(2, 0, 16, 0)
            };

            radioButton.CheckedChanged += OnRadioButtonCheckedChanged;
            RadioButtonGroup.Add(radioButton);
        }
    }


    void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = (RadioButton)sender;
        if (e.Value)
        {
            Value = Convert.ToInt32(radioButton.Value);
        }
    }

    #endregion

    public RadioButtons()
	{
		InitializeComponent();
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();

        // Subscribe to size changes so we can switch layout orientation on small screens
        this.SizeChanged -= RadioButtons_SizeChanged;
        this.SizeChanged += RadioButtons_SizeChanged;

        // Apply initial orientation
        UpdateOrientation(this.Width);
    }

    private void RadioButtons_SizeChanged(object? sender, EventArgs e)
    {
        UpdateOrientation(this.Width);
    }

    private void UpdateOrientation(double width)
    {
        // Threshold in device-independent units. If width is small, stack vertically.
        const double smallWidthThreshold = 480; // adjust as needed

        if (RadioButtonGroup == null) return;

        if (width > 0 && width <= smallWidthThreshold)
        {
            RadioButtonGroup.Orientation = StackOrientation.Vertical;
        }
        else
        {
            RadioButtonGroup.Orientation = StackOrientation.Horizontal;
        }
    }
}
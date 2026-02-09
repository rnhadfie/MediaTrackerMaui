using MauiApp1.Shared;
using System.Collections.Generic;

namespace MauiApp1.Front.Components.Shared;

public partial class RadioButtons : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(Shared.RadioButtons), "", BindingMode.TwoWay, null, OnLabelTextChanged);

    public static readonly BindableProperty groupNameProperty =
            BindableProperty.Create(nameof(GroupName), typeof(string), typeof(Shared.RadioButtons), "", BindingMode.TwoWay, null, OnGroupNameChanged);

    public static readonly BindableProperty SourceProperty =
           BindableProperty.Create(nameof(Source), typeof(TextValuePair<int>), typeof(Shared.RadioButtons), null, BindingMode.TwoWay, null, OnSoruceChanged);

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

    public object Source
    {
        get => GetValue(SourceProperty);
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
        control.radio_label.Text = newValue.ToString();
    }

    private static void OnGroupNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.RadioButtons)bindable;
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.RadioButtons)bindable;
    }

    private static void OnClassIdChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.RadioButtons)bindable;
    }

    private static void OnSoruceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Shared.RadioButtons)bindable;
    }

    public void AddData(List<TextValuePair<int>> Buttonsource)
    {
        if (Buttonsource != null)
        {
            foreach (var item in Buttonsource)
            {
                Label label = new Label();
                label.Text = item.Text;
                label.Padding = new Thickness(2, 0, 16, 0);
                var radioButton = new RadioButton
                {
                    Content = label,
                    Value = item.Value,
                    GroupName = this.GroupName,
                    IsChecked = (int)Value == item.Value,
                };
                radioButton.CheckedChanged += OnRadioButtonCheckedChanged;
                RadioButtonGroup.Add(radioButton);
            }
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
}
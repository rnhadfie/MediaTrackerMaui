using MauiApp1.BackEnd.Shared;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;

namespace MauiApp1.Front.Components.Shared;


public partial class RadioButtonGroup : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IList<TextValuePair<string, int>>),
        typeof(RadioButtonGroup),
        default(IList<TextValuePair<string, int>>),
        propertyChanged: OnItemsSourceChanged);

    public IList<TextValuePair<string, int>> ItemsSource
    {
        get => (IList<TextValuePair<string, int>>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create(
        nameof(SelectedValue),
        typeof(int?),
        typeof(RadioButtonGroup),
        default(int?),
        BindingMode.TwoWay,
        propertyChanged: OnSelectedValueChanged);

    public int? SelectedValue
    {
        get => (int?)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
        nameof(Header),
        typeof(string),
        typeof(RadioButtonGroup),
        string.Empty,
        propertyChanged: OnHeaderChanged);

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly BindableProperty IsEditProperty = BindableProperty.Create(
        nameof(IsEdit),
        typeof(bool),
        typeof(RadioButtonGroup),
        true,
        propertyChanged: OnIsEditChanged);

    public bool IsEdit
    {
        get => (bool)GetValue(IsEditProperty);
        set => SetValue(IsEditProperty, value);
    }

    public RadioButtonGroup()
    {
        InitializeComponent();
        BuildRadioButtons();
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldVal, object newVal)
    {
        if (bindable is RadioButtonGroup ctrl)
            ctrl.BuildRadioButtons();
    }

    private static void OnSelectedValueChanged(BindableObject bindable, object oldVal, object newVal)
    {
        if (bindable is RadioButtonGroup ctrl)
            ctrl.UpdateRadioButtons();
    }

    private static void OnHeaderChanged(BindableObject bindable, object oldVal, object newVal)
    {
        if (bindable is RadioButtonGroup ctrl)
        {
            ctrl.HeaderLabel.Text = newVal as string ?? string.Empty;
            ctrl.HeaderLabel.IsVisible = !string.IsNullOrWhiteSpace(ctrl.HeaderLabel.Text);
        }
    }

    private static void OnIsEditChanged(BindableObject bindable, object oldVal, object newVal)
    {
        if (bindable is RadioButtonGroup ctrl)
        {
            var isEdit = (bool)newVal;
            foreach (var rb in ctrl.RadioStack.Children.OfType<RadioButton>())
                rb.IsEnabled = isEdit;
        }
    }

    private void BuildRadioButtons()
    {
        RadioStack.Children.Clear();
        if (ItemsSource == null)
            return;

        foreach (var pair in ItemsSource)
        {
            var rb = new RadioButton
            {
                Content = pair.Text,
                Value = pair.Value,
                IsChecked = SelectedValue == pair.Value,
                IsEnabled = IsEdit,
                VerticalOptions = LayoutOptions.Center
            };
            rb.CheckedChanged += (s, e) =>
            {
                if (e.Value)
                    SelectedValue = (int)rb.Value;
            };
            RadioStack.Children.Add(rb);
        }
    }

    private void UpdateRadioButtons()
    {
        foreach (var rb in RadioStack.Children.OfType<RadioButton>())
        {
            rb.IsChecked = (int?)rb.Value == SelectedValue;
        }
    }
}
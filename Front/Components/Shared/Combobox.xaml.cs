using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Shared;
using Microsoft.Maui.Controls;

namespace MauiApp1.Front.Components.Shared;

public partial class Combobox : ContentView
{

    #region Bindable Properties
    public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(Combobox), "", BindingMode.TwoWay, null, OnPlaceHolderTextChanged);

    public static readonly BindableProperty InputListProperty =
           BindableProperty.Create(nameof(InputList), typeof(List<TextValuePair<int>>), typeof(Combobox), null, BindingMode.TwoWay, null, OnInputListChanged);

    public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(object), typeof(Combobox), "", BindingMode.TwoWay, null, OnValueChanged);

    public event EventHandler SelectionChanged;
    #endregion

    #region Properties


    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public List<TextValuePair<int>> InputList
    {
        get => (List<TextValuePair<int>>)GetValue(InputListProperty);
        set => SetValue(InputListProperty, value);
    }

    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #endregion

    #region Bindable Property changed methods

    private static void OnPlaceHolderTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Combobox)bindable;
        control.comboboxLabel.Text = (string)newValue;
    }

    private static void OnInputListChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Combobox)bindable;
        control.SeriesList.ItemsSource = (List<TextValuePair<int>>)newValue;
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Combobox)bindable;
        control.SearchEntry.Text = ((TextValuePair<int>)newValue).Text;
    }

    #endregion

    BookSetupViewModel _viewModel;
    BookController controller;
    public Combobox()
	{
		InitializeComponent();
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        // Filter items as the user types
        var oldList = SeriesList.ItemsSource;
        var filter = e.NewTextValue.ToLower();
        SeriesList.ItemsSource = InputList.Where(i => i.Text.ToLower().Contains(filter)).ToList();

        DropdownBorder.IsVisible = true;

        if (!string.IsNullOrEmpty(e.NewTextValue) && string.IsNullOrEmpty(e.OldTextValue))
        {
            AnimateLabel(true);
        }
        else if (string.IsNullOrEmpty(e.NewTextValue) && !string.IsNullOrEmpty(e.OldTextValue))
        {
            AnimateLabel(false);
        }

        Value = new TextValuePair<int>(e.NewTextValue, -1);
        
    }

    private async void OnEntryFocused(object sender, FocusEventArgs e)
    {
        DropdownBorder.IsVisible = true;

       await AnimateLabel(true);
    }

    private async void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        // Delay slightly so the SelectionChanged event can fire before the list disappears
        await Task.Delay(200);
        DropdownBorder.IsVisible = false;

        if (string.IsNullOrEmpty(comboboxLabel.Text))
        {
            await AnimateLabel(false);
        }
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is TextValuePair<int> selectedItem)
        {
            SearchEntry.Text = selectedItem.Text;
            DropdownBorder.IsVisible = false;

            Value = new TextValuePair<int>(selectedItem.Text, selectedItem.Value);
            SelectionChanged?.Invoke(this, EventArgs.Empty);
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
                comboboxLabel.TranslateTo(0, -20, length, easing),
                 comboboxLabel.ScaleTo(0.8, length, easing)
            );
        }
        else
        {
            // Animate label down and scale up
            await Task.WhenAll(
                comboboxLabel.TranslateTo(0, 0, length, easing),
                comboboxLabel.ScaleTo(1, length, easing)
            );
        }
    }
}
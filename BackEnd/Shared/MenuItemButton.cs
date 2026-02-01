namespace MauiApp1.Shared;

public class MenuItemButton : Button
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(object), typeof(MenuItemButton), null);

    public static readonly BindableProperty NavigationLinkProperty =
        BindableProperty.Create(nameof(Nav), typeof(object), typeof(MenuItemButton), null);

    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public object Nav
    {
        get => GetValue(NavigationLinkProperty);
        set => SetValue(NavigationLinkProperty, value);
    }
}
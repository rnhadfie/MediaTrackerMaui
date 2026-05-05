namespace MauiApp1.Front.Components.Menu;

public partial class MenuBar : ContentPage
{
	public MenuBar()
	{
		InitializeComponent();
	}

	// Fallback InitializeComponent to load XAML at runtime when generated code isn't present.
	private void InitializeComponent()
	{
		// Load the XAML associated with this class (no using required; fully-qualified call).
		Microsoft.Maui.Controls.Xaml.Extensions.LoadFromXaml(this, typeof(MenuBar));
	}
}
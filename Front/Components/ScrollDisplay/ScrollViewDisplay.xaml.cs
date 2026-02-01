using static MauiApp1.Shared.Enums;

namespace MauiApp1.Components.ScrollDisplay;

public partial class ScrollView : ContentView
{
	private DisplayOptions _options;
	public ScrollView()
	{
		InitializeComponent();
	}

	public ScrollView(DisplayOptions displayOptions)
	{
        _options = displayOptions;
        InitializeComponent();
    }
}
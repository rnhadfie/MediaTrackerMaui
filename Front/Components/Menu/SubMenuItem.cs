using static MauiApp1.Shared.Enums;

namespace MauiApp1.Components;

public partial class SubMenuItem : ContentView
{
	public SubMenuItem()
	{
		InitializeComponent();
	}

    private MediaDataType _mediaType;
    
    public SubMenuItem(MediaDataType type)
    {
        InitializeComponent();
        _mediaType =   type;
    }

    private void onUseBarcode() {
        Console.WriteLine("barcode");
    }

    private void onSearchIsbn()
    {
        Console.WriteLine("isbn");
    }

    private void onEnterManually()
    {
        Console.WriteLine("manual");
    }
}
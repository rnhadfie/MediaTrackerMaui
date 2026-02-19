using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Components.ScrollDisplay;
using MauiApp1.Shared;
using Microsoft.Maui.Controls;

namespace MauiApp1.Front.Components.Shared;

public partial class ItemGrid : ContentView
{
    #region Bindable Properties

    public static readonly BindableProperty InputListProperty =
           BindableProperty.Create(nameof(InputList), typeof(List<DisplayViewItem>), typeof(ItemGrid), null, BindingMode.TwoWay, null, OnInputListChanged);


    #endregion

    #region Properties

    public List<DisplayViewItem> InputList
    {
        get => (List<DisplayViewItem>)GetValue(InputListProperty);
        set => SetValue(InputListProperty, value);
    }

    private int CellSize = 160;

    private double LastWidth = -1;
    private double LastHeight = -1;

    #endregion

    #region Bindable Property changed methods

    private static void OnInputListChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ItemGrid)bindable;
        control.InputList = (List<DisplayViewItem>)newValue ?? new List<DisplayViewItem>();
        control.UpdateGrid();
    }


    #endregion

    public ItemGrid()
	{
		InitializeComponent();
        UpdateGrid();
        //ItemGridView.SizeChanged += OnGridSizeChanged;

    }
    private void OnGridSizeChanged(object? sender, EventArgs e)
    {
        UpdateGrid();
    }

    private void AddGridContent(int rows, int cols)
    {
        int totalList = InputList != null ? InputList?.Count ?? -1 : -1;
        int currentNum = 0;
        // Example: Add a Label to each cell
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (currentNum < totalList)
                {
                    DisplayViewItem viewItem = InputList[currentNum++];
                    var scrollViewItem = new ScrollViewItem
                    {
                        MediaType = viewItem.Type,
                        Source = viewItem.Cover ?? [],
                        LabelText = viewItem.Name,
                        ItemId = viewItem.Id,
                    };
                    // Set the position of the label in the grid
                    Grid.SetRow(scrollViewItem, r);
                    Grid.SetColumn(scrollViewItem, c);
                    ItemGridView.Children.Add(scrollViewItem);
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void UpdateGrid()
    {
        ItemGridView.RowDefinitions.Clear();
        ItemGridView.ColumnDefinitions.Clear();
        ItemGridView.Children.Clear();

        // Prefer measured sizes; fallback to Window only if available
        double width = (ItemGridView?.Width > 0) ? ItemGridView.Width :
                       (this.Width > 0) ? this.Width :
                       (DeviceDisplay.Current.MainDisplayInfo.Width);

        double height = (ItemGridView?.Height > 0) ? ItemGridView.Height :
                        (this.Height > 0) ? this.Height :
                        (DeviceDisplay.Current.MainDisplayInfo.Height);


        // If we don't have valid sizes yet, wait for layout (SizeChanged will call UpdateGrid)
        if (width <= 0 || height <= 0) return;

        int numRows = Math.Max(1, (int)Math.Floor(height / CellSize));
        // Fix precedence: subtract 200 first, then divide by CellSize
        int numCols = Math.Max(1, (int)Math.Floor((width - 200) / CellSize));

        for (int i = 0; i < numRows; i++)
        {
            ItemGridView?.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        }

        for (int i = 0; i < numCols; i++)
        {
            ItemGridView?.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        AddGridContent(numRows, numCols);
    }

}
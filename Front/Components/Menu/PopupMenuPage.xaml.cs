using CommunityToolkit.Maui.Views;
using MauiApp1.BackEnd.Database;
using MauiApp1.Components.Books;
using MauiApp1.Front.Components.Series;
using MauiApp1.Front.Components.Video;
using MauiApp1.Shared;
using System.Threading.Tasks;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Components;

public partial class PopupMenuPage
{
    private readonly DataContext _dbContext;

    public PopupMenuPage(DataContext dbContext)
	{
		InitializeComponent();
        AddMenus();
        _dbContext = dbContext;
    }

    public async void onMenuSelected(object sender, EventArgs e)
    {
        var currentBtn = (MenuItemButton)sender;
        MediaDataType mediaDataType = currentBtn != null && currentBtn.Value != null ? (MediaDataType)currentBtn.Value : MediaDataType.All;

        if (MediaDataType.Book == mediaDataType) {
            await Shell.Current.GoToAsync(nameof(AddBook));
        }

        if (MediaDataType.Series == mediaDataType)
        {
            await Shell.Current.GoToAsync(nameof(AddSeries));
        }

        if (MediaDataType.Video == mediaDataType)
        {
            await Shell.Current.GoToAsync(nameof(AddVideo));
        }
    }

    public void onBackMenuClicked(object sender, EventArgs e)
    {
        AddMenus();
    }


    public void AddMenus()
    {
        MenuLayout.Children.Clear();
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Series));
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Book));
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Video));
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Cd));

    }

    public MenuItemButton CreateMenuItem(MediaDataType type)
    {
        string text = string.Empty;
        string classId = string.Empty;
        string imageSource = string.Empty;

        switch (type)
        {
            case MediaDataType.Book:
                text = "Add Book";
                classId = "Book";
                imageSource = "icons_book_32.png";
                break;
            case MediaDataType.Video:
                text = "Add DVD/Blu-ray";
                classId = "Video";
                imageSource = "icons_dvd_32.png";
                break;
            case MediaDataType.Cd:
                text = "Add CD";
                classId = "Cd";
                imageSource = "icons_cd_50.png";
                break;
            case MediaDataType.Series:
                text = "Add Series";
                classId = "series";
                imageSource = "icons_cd_50.png";
                break;
        }
        MenuItemButton button = new()
        {
            Text = text,
            Value = type,
            ClassId = classId,
            BackgroundColor = Color.FromArgb("#FFFFFF"),
            TextColor = Color.FromArgb("#000000"),
            BorderColor = Color.FromArgb("#000000"),
            BorderWidth = 1,
            HorizontalOptions=LayoutOptions.Fill,
            ImageSource = imageSource

        };
        button.Clicked += onMenuSelected;

        return button;
    }
}
using CommunityToolkit.Maui.Views;
using MauiApp1.BackEnd.Database;
using MauiApp1.Front.Components.Books;
using MauiApp1.Front.Components.Music;
using MauiApp1.Front.Components.Other;
using MauiApp1.Front.Components.Collections;
using MauiApp1.Front.Components.Video;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Components.Menu;

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
            await Shell.Current.GoToAsync($"{nameof(BookForm)}?Add={true}&BookId={-1}");
        }

        if (MediaDataType.Collection == mediaDataType)
        {
            await Shell.Current.GoToAsync($"{nameof(CollectionForm)}?Add={true}&SeriesId={-1}");
        }

        if (MediaDataType.Video == mediaDataType)
        {
            await Shell.Current.GoToAsync($"{nameof(VideoForm)}?Add={true}&VideoId={-1}");
        }

        if (MediaDataType.Cd == mediaDataType)
        {
            await Shell.Current.GoToAsync($"{nameof(MusicForm)}?Add={true}&Id={-1}");
        }

        if (MediaDataType.Other == mediaDataType)
        {
            await Shell.Current.GoToAsync($"{nameof(OtherForm)}?Add={true}&Id={-1}");
        }
    }

    public void onBackMenuClicked(object sender, EventArgs e)
    {
        AddMenus();
    }


    public void AddMenus()
    {
        MenuLayout.Children.Clear();
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Collection));
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Book));
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Video));
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Cd));
        MenuLayout.Children.Add(CreateMenuItem(MediaDataType.Other));

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
            case MediaDataType.Other:
                text = "Add Other Items";
                classId = "Other";
                imageSource = "icons_cd_50.png";
                break;
            case MediaDataType.Collection:
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
            HorizontalOptions = LayoutOptions.Fill,
            ImageSource = imageSource,
            Margin = new Thickness(8,8,8,8)

        };
        button.Clicked += onMenuSelected;

        return button;
    }
}
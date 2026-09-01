using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiApp1.Front.Components.Videos.ViewModels;

public partial class VideoHomeViewModel : ObservableObject
{
    private readonly VideoController _controller = new VideoController();

    public VideoHomeViewModel()
    {
        // Load asynchronously but don't block constructor
        _ = LoadSetupAsync();
    }

    public async Task LoadSetupAsync()
    {
        var setup = await _controller.GetVideoSetup();
        VideoSetup = setup;
        var types = setup.Type;
        var all = new MauiApp1.BackEnd.Shared.TextValuePair<string, int>("All", 0);
        types.Insert(0, all);
        TypeOptions = new ObservableCollection<MauiApp1.BackEnd.Shared.TextValuePair<string, int>>(setup.Type);
        SelectedType = TypeOptions.FirstOrDefault();

        var source = VideoSetup?.VideoSeries ?? Enumerable.Empty<VideoItem>();
        if (SelectedType == null || SelectedType.Value == 0)
        {
            listOfVideoSeries = new ObservableCollection<VideoItem>(source);
        }
        UpdateListOfVideos();
    }

    private void UpdateListOfVideos()
    {
        var source = VideoSetup?.Video ?? Enumerable.Empty<MauiApp1.BackEnd.Models.Video.Video>();
        if (SelectedType == null || SelectedType.Value == 0)
        {
            ListOfVideos = new ObservableCollection<MauiApp1.BackEnd.Models.Video.Video>(source);
        }
        else
        {
            var filtered = source.Where(x => (int)x.Type == SelectedType.Value);
            ListOfVideos = new ObservableCollection<MauiApp1.BackEnd.Models.Video.Video>(filtered);
        }
    }

    partial void OnSelectedTypeChanged(MauiApp1.BackEnd.Shared.TextValuePair<string, int> value)
    {
        UpdateListOfVideos();
    }

    [ObservableProperty]
    private ObservableCollection<MauiApp1.BackEnd.Shared.TextValuePair<string, int>> typeOptions;

    [ObservableProperty]
    private MauiApp1.BackEnd.Shared.TextValuePair<string, int> selectedType;

    [ObservableProperty]
    private BackEnd.Controllers.ViewModels.VideoSetupViewModel videoSetup;

    [ObservableProperty]
    private ObservableCollection<MauiApp1.BackEnd.Models.Video.Video> listOfVideos;

    [ObservableProperty]
    private ObservableCollection<MauiApp1.BackEnd.Models.Video.VideoItem> listOfVideoSeries;
}

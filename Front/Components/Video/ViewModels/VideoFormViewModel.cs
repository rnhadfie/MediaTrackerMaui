using MauiApp1.BackEnd.Models.Video;
using MauiApp1.BackEnd.Shared;
using System.Collections.ObjectModel;
using System.ComponentModel;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Videos.ViewModels;

public class VideoFormViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public VideoFormViewModel()
    {
        Id.PropertyChanged += (_, __) => Notify(nameof(Id));
        Name.PropertyChanged += (_, __) => Notify(nameof(Name));
        Watched.PropertyChanged += (_, __) => Notify(nameof(Watched));
    }

    public MauiApp1.BackEnd.Models.Video.Video ToModel()
    {
        return new MauiApp1.BackEnd.Models.Video.Video
        {
            Id = Id.Value,
            Name = Name.Value,
            Type = (Enums.VideoType)SelectedType.Value,
            Format = (Enums.VideoFormat)SelectedFormat.Value,
            SeriesId = SelectedVideoSeries.Value?.Value ?? 0,
            Watched = Watched.Value
        };
    }

    public Observable<int> Id { get; set; } = new Observable<int>(0);
    public Observable<string> Name { get; set; } = new Observable<string>(string.Empty);
    public Observable<TextValuePair<string, int>?> SelectedVideoSeries { get; set; } = new Observable<TextValuePair<string, int>?>(null);
    public Observable<int> SelectedFormat { get; set; } = new Observable<int>(0);
    public Observable<int> SelectedType { get; set; } = new Observable<int>(0);
    public Observable<bool> Watched { get; set; } = new Observable<bool>(false);
    public Observable<string> NewSeriesName { get; set; } = new Observable<string>(string.Empty);
    public bool EnableNewSeries => SelectedVideoSeries.Value == null;

    public ObservableCollection<int> Genre { get; set; } = new ObservableCollection<int>();
    public Observable<int> Format { get; set; } = new Observable<int>(0);
    public Observable<int?> Season { get; set; } = new Observable<int?>(null);
    public Observable<bool> Read { get; set; } = new Observable<bool>(false);
    public Observable<bool> IsEdit { get; set; } = new Observable<bool>(true);

    // Selected items from multi-picker (stores selected option objects)
    public List<TextValuePair<string, int>> SelectedGenres { get; set; } = new List<TextValuePair<string, int>>();



    // When a picker selects a TextValuePair, store it here so SelectedItem binding has correct type
    public Observable<int> SeriesId { get; set; } = new Observable<int>(0);

    public Observable<bool> IsSeriesSelected { get; set; } = new Observable<bool>(false);
    public Observable<VideoType?> Type { get; set; } = new Observable<VideoType?>(null);

    public Observable<string?> NewVideoSeriesName { get; set; } = new Observable<string?>(null);

    public List<TextValuePair<string, int>> VideoSeriesList { get; set; } = new List<TextValuePair<string, int>>();

    public ObservableCollection<TextValuePair<string, int>> GenreOptions { get; set; } = new ObservableCollection<TextValuePair<string, int>>();
}

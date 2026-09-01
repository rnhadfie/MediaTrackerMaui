
using MauiApp1.BackEnd.Shared;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MauiApp1.BackEnd.Controllers.ViewModels;

namespace MauiApp1.Front.Components.Videos.ViewModels;

public class VideoSeriesFormViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public VideoSeriesFormViewModel()
    {
        // Subscribe to observables we depend on so computed    properties update UI
        Id.PropertyChanged += (_, __) => Notify(nameof(Id));
        Title.PropertyChanged += (_, __) => Notify(nameof(Title));
        Ongoing.PropertyChanged += (_, __) => Notify(nameof(Ongoing));
        Collecting.PropertyChanged += (_, __) => Notify(nameof(Collecting));
        UpToDateComplete.PropertyChanged += (_, __) => Notify(nameof(UpToDateComplete));
        IsEdit.PropertyChanged += (_, __) => Notify(nameof(IsEdit));

    }

    public Observable<int> Id { get; set; } = new Observable<int>(0);
    public Observable<string> Title { get; set; } = new Observable<string>(string.Empty);

    public Observable<bool> Ongoing { get; set; } = new Observable<bool>(false);

    public Observable<string> Collecting { get; set; } = new Observable<string>("");

    public Observable<bool> UpToDateComplete { get; set; } = new Observable<bool>(false);

    public Observable<string> Parent { get; set; } = new Observable<string>("");
    public Observable<bool> IsEdit { get; set; } = new Observable<bool>(true);

    // Genre selections
    public ObservableCollection<int> Genre { get; set; } = new ObservableCollection<int>();

    private ObservableCollection<TextValuePair<string, int>> _genreOptions = new ObservableCollection<TextValuePair<string, int>>();
    public ObservableCollection<TextValuePair<string, int>> GenreOptions
    {
        get => _genreOptions;
        set
        {
            if (!ReferenceEquals(_genreOptions, value))
            {
                _genreOptions = value ?? new ObservableCollection<TextValuePair<string, int>>();
                Notify(nameof(GenreOptions));
            }
        }
    }

    // Additional setup collections
    public ObservableCollection<TextValuePair<string, int>> Format { get; set; } = new ObservableCollection<TextValuePair<string, int>>();
    public ObservableCollection<TextValuePair<string, int>> Type { get; set; } = new ObservableCollection<TextValuePair<string, int>>();
    public ObservableCollection<TextValuePair<string, int>> Langauge { get; set; } = new ObservableCollection<TextValuePair<string, int>>();

    // Load setup data (genres, formats, etc.) into the view model
    public async Task LoadSetupAsync()
    {
        var controller = new VideoController();
        var setup = await controller.GetVideoSetup();
        if (setup?.Genre != null)
        {
            GenreOptions = new ObservableCollection<TextValuePair<string, int>>(setup.Genre);
        }
        if (setup?.Format != null)
            Format = new ObservableCollection<TextValuePair<string, int>>(setup.Format);
        if (setup?.Type != null)
            Type = new ObservableCollection<TextValuePair<string, int>>(setup.Type);
        if (setup?.Langauge != null)
            Langauge = new ObservableCollection<TextValuePair<string, int>>(setup.Langauge);
    }
}

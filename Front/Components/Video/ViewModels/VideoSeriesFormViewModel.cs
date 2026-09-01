
using MauiApp1.BackEnd.Shared;
using System.ComponentModel;

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
}

using MauiApp1.BackEnd.Shared;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Books.ViewModels
{
    public class BookSeriesFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public BookSeriesFormViewModel()
        {
            // Subscribe to observables we depend on so computed    properties update UI
            Id.PropertyChanged += (_, __) => Notify(nameof(Id));
            Title.PropertyChanged += (_, __) => Notify(nameof(Title));
            Author.PropertyChanged += (_, __) => Notify(nameof(Author));
            Artist.PropertyChanged += (_, __) => Notify(nameof(Artist));
            Ongoing.PropertyChanged += (_, __) => Notify(nameof(Ongoing));
            Collecting.PropertyChanged += (_, __) => Notify(nameof(Collecting));
            UpToDateComplete.PropertyChanged += (_, __) => Notify(nameof(UpToDateComplete));
            IsEdit.PropertyChanged += (_, __) => Notify(nameof(IsEdit));


        }

        public Observable<int> Id { get; set; } = new Observable<int>(0);
        public Observable<string> Title { get; set; } = new Observable<string>(string.Empty);
        public Observable<string?> Author { get; set; } = new Observable<string?>(null);
        public Observable<string?> Artist { get; set; } = new Observable<string?>(null);

        public Observable<bool> Ongoing { get; set; } = new Observable<bool>(false);

        public Observable<bool> IsEdit { get; set; } = new Observable<bool>(true);

        public Observable<string> Collecting { get; set; } = new Observable<string>("");

        public Observable<bool> UpToDateComplete { get; set; } = new Observable<bool>(false);

        public Observable<int> TotalVolumes { get; set; } = new Observable<int>(0);

        public Observable<string> Parent { get; set; } = new Observable<string>("");
    }
}


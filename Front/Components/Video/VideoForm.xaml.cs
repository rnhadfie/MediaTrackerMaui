using DocumentFormat.OpenXml.Bibliography;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Controllers;
using MauiApp1.Front.Components.Books.ViewModels;
using MauiApp1.Front.Components.Videos.ViewModels;
using System.Collections.ObjectModel;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Videos;

[QueryProperty(nameof(id), "id")] // For objects
[QueryProperty(nameof(edit), "edit")]
public partial class VideoForm : ContentPage, IQueryAttributable
{
    private int _id;
    private bool _edit;
    public int id;
    public bool edit = true;
    // removed duplicate public fields 'public int id; public bool edit = true;'
    private readonly VideoController _controller;

    public VideoForm()
    {
        InitializeComponent();
        _id = _id; // or keep as-is if you expect QueryProperties to set later
        _edit = _edit;
        _controller = new VideoController();
    }

    private async Task LoadVideo(int id)
    {
        // kept for synchronous callers - call the async variant where possible
        var video = await _controller.GetVideo(id);

        if (video == null) return;

        var vm = (BindingContext as VideoFormViewModel) ?? new VideoFormViewModel();

        // Set the Value on existing Observable<T> instances so bindings remain intact
        vm.Id.Value = video.Id.Value;
        vm.Name.Value = video.Name;
        vm.Season.Value = video.Season;
        vm.Genre = new ObservableCollection<int>(video.Genre?.Select(g => (int)g) ?? new List<int>());
        vm.Format.Value = (int)video.Format;
        vm.Watched.Value = video.Watched;
        //vm.Language.Value = new ObservableCollection<int>(video.Language?.Select(g => (int)g) ?? new List<int>()); ;
        //vm.SelectedVideoSeries.Value = video.SeriesId;
        vm.Type.Value = video.Type;

        vm.IsEdit = _edit;

        BindingContext = vm;

        VideoGenrePicker.SelectedValues = vm.Genre;
        //VideoLanguageGroup.SelectedItem = vm.Language;
        VideoFormatGroup.SelectedValue = vm.Format;
    }

    private void DisableFields(bool isEnabled)
    {
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // load setup and book data asynchronously and update ViewModel
        var setup = await _controller.GetVideoSetup();
        await LoadVideo(_id);

        var vm = (BindingContext as VideoFormViewModel) ?? new VideoFormViewModel();

        // populate lists
        vm.VideoSeriesList = setup.VideoSeries?.Select(s => new TextValuePair<string, int>(s.Title, s.Id)).ToList() ?? new List<TextValuePair<string, int>>();

        vm.GenreOptions = setup.Genre != null
            ? setup.Genre
            : new ObservableCollection<TextValuePair<string, int>>();

        VideoSeriesPicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.VideoSeriesList);
        VideoGenrePicker.ItemsSource = new ObservableCollection<TextValuePair<string, int>>(vm.GenreOptions);

        List<TextValuePair<string, int>> formats = new List<TextValuePair<string, int>>();
        foreach (var item in setup.Format)
        {
            UraniumUI.Material.Controls.RadioButton radioButton = new UraniumUI.Material.Controls.RadioButton();
            formats.Add(new TextValuePair<string, int>(item.Text, item.Value));
        }
        VideoFormatGroup.ItemsSource = formats;

        if (_id > 0)
        {
            var video = await _controller.GetVideoAsync(_id);
            if (video != null)
            {
                vm.Id.Value = video.Id.Value;
                vm.Name.Value = video.Name;
                vm.Season.Value = video.Season;
                vm.Format.Value = (int)video.Format;
                vm.Read.Value = video.Watched;
                vm.SeriesId.Value = video.SeriesId;
                vm.Type.Value = video.Type;
            }
        }

        BindingContext = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null)
            return;

        if (query.TryGetValue("edit", out var editObj) || query.TryGetValue("Edit", out editObj))
        {
            if (editObj is string editStr && bool.TryParse(editStr, out var editBool))
            {
                _edit = editBool;
            }
            else if (editObj is bool editBoolVal)
            {
                _edit = editBoolVal;
            }
            DisableFields(_edit);
        }

        if (query.TryGetValue("id", out var idObj) || query.TryGetValue("Id", out idObj))
        {
            int parsed = 0;
            if (idObj is string s && int.TryParse(s, out parsed))
            {
                _id = parsed;
            }
            else if (idObj is int i)
            {
                _id = i;
            }

            if (_id > 0)
            {
                _ = LoadVideo(_id);
            }
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("bookhome");
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        // Handle save
        var vm = BindingContext as VideoFormViewModel;
        if (vm == null)
        {
            await DisplayAlert("Error", "Unable to read form data.", "OK");
            return;
        }

        try
        {
            var video = new Video
            {
                Id = vm.Id.Value,
                Name = vm.Name.Value,
                Season = vm.Season.Value ?? -1,
                Format = (VideoFormat)vm.Format.Value,
                Watched = vm.Read.Value,
                Type = (VideoType)vm.Type.Value
            };

            if (vm.SelectedGenres != null && vm.SelectedGenres.Count > 0)
            {
                video.Genre = vm.SelectedGenres.Select(g => g.Value).ToList();
            }
            else
            {
                video.Genre = new List<int>();
            }

            var result = await _controller.SaveVideoAsync(video, vm.NewVideoSeriesName);
            if (result)
            {
                await DisplayAlert("Saved", "Video saved successfully.", "OK");
                await Shell.Current.GoToAsync("videohome");
            }
            else
            {
                await DisplayAlert("Error", "Failed to save video.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Save failed: {ex.Message}", "OK");
        }
    }
}

using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Models;
using MauiApp1.BackEnd.Models.Video;
using MauiApp1.BackEnd.Shared;
using MauiApp1.Controllers;
using MauiApp1.Front.Components.Books.ViewModels;
using MauiApp1.Front.Components.Videos.ViewModels;

namespace MauiApp1.Front.Components.Videos;

[QueryProperty(nameof(id), "id")] // For objects
[QueryProperty(nameof(edit), "edit")]
public partial class VideoSeriesForm : ContentPage
{
    private VideoSeriesFormViewModel _vm;
    private VideoController _controller;



    private int _id;
    private bool _edit;
    public int id;
    public bool edit = true;

    public VideoSeriesForm(VideoSeriesFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
        _id = id;
        _edit = edit;
        _controller = new VideoController();
    }

    private async
    Task
    LoadBook(int id)
    {
        // kept for synchronous callers - call the async variant where possible
        var videoSeries = await _controller.GetVideoSeriesAsync(id);

        if (videoSeries == null) return;

        var vm = (BindingContext as VideoSeriesFormViewModel) ?? new VideoSeriesFormViewModel();

        // Set the Value on existing Observable<T> instances so bindings remain intact
        vm.Id.Value = videoSeries.Id;
        vm.Title.Value = videoSeries.Title;
        vm.Collecting.Value = videoSeries.Collecting;
        vm.Ongoing.Value = videoSeries.Ongoing;
        vm.Parent.Value = videoSeries.Parent;


        // Ensure the viewmodel's edit mode matches the parsed query value
        vm.IsEdit.Value = _edit;

        BindingContext = vm;
    }

    private void DisableFields(bool isEnabled)
    {
        VideoSeriesTitle.IsEnabled = isEnabled;
        VideoSeriesCollecting.IsEnabled = isEnabled;
        VideoSeriesParent.IsEnabled = isEnabled;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // load setup and book data asynchronously and update ViewModel
        var setup = await _controller.GetVideoSetup();
        await LoadBook(_id);

        var vm = (BindingContext as VideoSeriesFormViewModel) ?? new VideoSeriesFormViewModel();



        List<TextValuePair<string, int>> formats = new List<TextValuePair<string, int>>();
        foreach (var item in setup.Format)
        {
            UraniumUI.Material.Controls.RadioButton radioButton = new UraniumUI.Material.Controls.RadioButton();
            formats.Add(new TextValuePair<string, int>(item.Text, item.Value));
        }

        foreach (var item in setup.Langauge)
        {
            UraniumUI.Material.Controls.RadioButton radioButton = new UraniumUI.Material.Controls.RadioButton();
            radioButton.Text = item.Text;
            radioButton.Value = item.Value;
            //BookLanguageGroup.Children.Add(radioButton);
        }

        // If an id was provided, load the book and map genre selections
        if (_id > 0)
        {
            var videoSeries = await _controller.GetVideoSeriesAsync(_id);
            if (videoSeries != null)
            {
                vm.Id.Value = videoSeries.Id;
                vm.Title.Value = videoSeries.Title;
                vm.Collecting.Value = videoSeries.Collecting;
                vm.Ongoing.Value = videoSeries.Ongoing;
                vm.Parent.Value = videoSeries.Parent;
            }
        }

        BindingContext = vm;
    }

    // IQueryAttributable implementation - called when navigating with Shell query parameters
    // Example navigation: Shell.Current.GoToAsync($"bookform?id={id}");
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
                LoadBook(_id);
            }
        }


    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        // Navigate back to the Videos home (series are embedded there)
        await Shell.Current.GoToAsync("videohome");
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        // Handle save
        var vm = BindingContext as VideoSeriesFormViewModel;
        if (vm == null)
        {
            await DisplayAlert("Error", "Unable to read form data.", "OK");
            return;
        }

        try
        {
            var videoSeries = new VideoSeries
            {
                Id = vm.Id.Value,
                Title = vm.Title.Value,
                Collecting = vm.Collecting.Value,
                Ongoing = vm.Ongoing.Value,
                Parent = vm.Parent.Value,
                UpToDateComplete = vm.UpToDateComplete.Value,
            };




            var result = await _controller.SaveVideoSeriesAsync(videoSeries);
            if (result > 0)
            {
                await DisplayAlert("Saved", "Video series saved successfully.", "OK");
                await Shell.Current.GoToAsync("videohome");
            }
            else
            {
                await DisplayAlert("Error", "Failed to save video series.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Save failed: {ex.Message}", "OK");
        }
    }
}

using DocumentFormat.OpenXml.Spreadsheet;
using MauiApp1.BackEnd.Models;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Front.Components.Books.ViewModels;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using UraniumUI.Controls;
using UraniumUI.Dialogs;
using static MauiApp1.BackEnd.Shared.Enums;
using Color = Microsoft.Maui.Graphics.Color;
using Colors = Microsoft.Maui.Graphics.Colors;

namespace MauiApp1.Front.Components.Books;

public partial class BookHome : ContentPage
{
    private readonly IDialogService _dialogService;

    private async void ShowDialog_Clicked(object sender, EventArgs e)
    {
        
    }

    public BookHome(BookHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        BookTypeDropDown.ItemDisplayBinding = new Binding("Text");

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            // If the embedded series list exists, load its data
            if (BookSeriesListView != null)
            {
                await BookSeriesListView.LoadSeriesAsync();
            }
        }
        catch
        {
            // ignore load errors here
        }
    }
    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        // Let the user choose whether to add a single item or a series
        var choice = await Application.Current.MainPage.DisplayActionSheet("Add", "Cancel", null, "Item", "Series");
        if (string.IsNullOrEmpty(choice) || choice == "Cancel") return;
        if (choice == "Item")
        {
            await Shell.Current.GoToAsync($"/BookForm?id=0&edit=true");
        }
        else if (choice == "Series")
        {
            await Shell.Current.GoToAsync("/BookSeriesForm?edit=true");
        }
    }

    private async void OpenSeriesManager_Clicked(object sender, EventArgs e)
    {
        // Redirect to the Books home route which contains the embedded series list
        await Shell.Current.GoToAsync("books");
    }
   
    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Absolute navigation back to Home (root) using Shell
        await Shell.Current.GoToAsync("MainPage");
    }

    private async void OnAddBookClicked(object sender, EventArgs e)
    {
        // Navigate to BookForm page (assumes route or direct page exists)
        await Shell.Current.GoToAsync("/BookForm?id=0&edit=true");
    }

    private void BookTypeDropDown_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {

    }
}
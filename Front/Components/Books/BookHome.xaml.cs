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
   
    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Absolute navigation back to Home (root) using Shell
        await Shell.Current.GoToAsync("MainPage");
    }

    private async void OnAddBookClicked(object sender, EventArgs e)
    {
        // Navigate to BookForm page (assumes route or direct page exists)
        await Shell.Current.GoToAsync("/BookForm");
    }

    private void BookTypeDropDown_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {

    }
}
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service.Modals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

namespace MauiApp1.Front.Components.Books;


public partial class BookView : ContentPage
{

    private SeriesController _seriesViewController;
    private readonly DataContext _dbContext;
    private List<DisplayViewItem> _SeriesList;
    public BookView(DataContext dataContext)
	{
		
    }
}
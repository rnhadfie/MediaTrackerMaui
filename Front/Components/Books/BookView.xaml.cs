
using MauiApp1.BackEnd.Controllers;
using MauiApp1.BackEnd.Controllers.ViewModels;
using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Service.Modals;
using MauiApp1.Components;
using MauiApp1.Components.Books;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using MauiApp1.Front.Components.Series;
using MauiApp1.Service.Modals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System.Linq;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Front.Components.Books;

public partial class BookView : ContentPage
{
    BookSetupViewModel _viewModel;
    List<Book> _books;
    private BookController controller;
    private ScrollViewController _scrollViewController;
    private readonly DataContext _dbContext;
    public BookView(DataContext dataContext)
    {
        InitializeComponent();
        _dbContext = dataContext;
        controller = new BookController(_dbContext);
        _scrollViewController = new ScrollViewController(_dbContext);

        BookFilter bookFilter = new BookFilter() {
            Type = null,
            Take = null,
            Genre = null,
            Series = null,
            Format = null,
        };
        #region Setup 
        _books = controller.GetAllBooks(bookFilter);
        _viewModel = controller.GetBookSetup();

        BookView_All.Source = GetListOfDisplayViewItems(_books, 10);
        BookView_All.LocationText = $"{nameof(BookDetailView)}?BookType={-1}&BookFormat={-1}";

        List<Book> ebookList = _books.Where<Book>(x => x.Format == (int)BookFormat.EBook).ToList();
        BookView_EBooks.Source = GetListOfDisplayViewItems(ebookList, 10);
        BookView_EBooks.LocationText = $"{nameof(BookDetailView)}?BookType={-1}&BookFormat={BookFormat.EBook}";

        List<Book> novelList = _books.Where<Book>(x => x.Type == (int)BookType.Novel).ToList();
        BookView_Novels.Source = GetListOfDisplayViewItems(novelList, 10);
        BookView_Novels.LocationText = $"{nameof(BookDetailView)}?BookType={(int)BookType.Novel}&BookFormat={-1}";

        List<Book> lightNovelList = _books.Where<Book>(x => x.Type == (int)BookType.LightNovel).ToList();
        BookView_LightNovels.Source = GetListOfDisplayViewItems(ebookList, 10);
        BookView_LightNovels.LocationText = $"{nameof(BookDetailView)}?BookType={(int)BookType.LightNovel}&BookFormat={-1}";

        List<Book> mangaList = _books.Where<Book>(x => x.Type == (int)BookType.Manga).ToList();
        BookView_Manga.Source = GetListOfDisplayViewItems(mangaList, 10);
        BookView_Manga.LocationText = $"{nameof(BookDetailView)}?BookType={(int)BookType.Manga}&BookFormat={-1}";


        BookView_Series.Source = _scrollViewController.GetSeriesList().FindAll(x=>x.Type == MediaDataType.All || x.Type == MediaDataType.Book);
        //BookView_Series.LocationText = $"{nameof(BookDetailView)}?BookType={BookType.Manga}&BookFormat={-1}";

        #endregion
    }


    private List<DisplayViewItem> GetListOfDisplayViewItems(List<Book> books, int take)
    {

        List<Book> finalList = books.Take<Book>(10).ToList();
        return finalList.Select(x =>
        {
            return new DisplayViewItem
            {
                Type = MediaDataType.Book,
                Cover = x.Cover,
                Id = x.Id,
                Name = x.Title
            };
        }).ToList<DisplayViewItem>();

    }

    private async void OnOpenMenu(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(BookForm)}?Add={true}&BookId={-1}");
    }


}
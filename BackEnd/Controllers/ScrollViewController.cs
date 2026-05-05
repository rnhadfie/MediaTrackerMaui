using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.BackEnd.Service;

namespace MauiApp1.BackEnd.Controllers
{
    public class ScrollViewController
    {
        DataContext _dataContext;
        private SeriesService _SeriesService;
        private BookService _BookService;
        private VideoService _VideoService;

        public ScrollViewController(DataContext dataContext)
        {
            _dataContext = dataContext;
            _SeriesService = new Lazy<SeriesService>(() =>
            {
                return new SeriesService(_dataContext);
            }).Value;

            _BookService = new Lazy<BookService>(() =>
            {
                return new BookService(_dataContext);
            }).Value;

            _VideoService = new Lazy<VideoService>(() =>
            {
                return new VideoService(_dataContext);
            }).Value;
        }

        public List<DisplayViewItem> GetSeriesList()
        {
            return _SeriesService.GetDisplaySeriesList();
        }

        public List<DisplayViewItem> GetSeriesList(int take)
        {
            return _SeriesService.GetDisplaySeriesList(take);
        }

        public List<DisplayViewItem> GetBookList(int take)
        {
            return _BookService.GetAllBooks(take);
        }

        public List<DisplayViewItem> GetVideoList(int take)
        {
            return _VideoService.GetVideos(take);
        }
    }
}

    

using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.Controllers;
using MauiApp1.Controllers.ViewModels;
using System.Threading.Tasks;

namespace MauiApp1.Front.Components.Books.ViewModels
{
    public partial class BookSystemViewModel : ObservableObject
    {
        public BookSystemViewModel()
        {
            // Load data asynchronously and assign to Setup when available
            var controller = new BookController();
            controller.GetBooksAsync().ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Setup = task.Result;
                }
                else
                {
                    // handle errors if desired
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        [ObservableProperty]
        private BookSetupViewModel setup;
    }
}

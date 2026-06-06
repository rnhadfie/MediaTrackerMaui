using CommunityToolkit.Maui.Storage;
using MauiApp1.BackEnd.Controllers;
using MauiApp1.Front.Components.Books;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();


            ShellRouting.RegisterRouteSafe(nameof(MainPage), typeof(MainPage));
            /*
            ShellRouting.RegisterRouteSafe(nameof(CollectionForm), typeof(CollectionForm));
            ShellRouting.RegisterRouteSafe(nameof(CollectionDetailView), typeof(CollectionDetailView));
            ShellRouting.RegisterRouteSafe(nameof(CollectionView), typeof(CollectionView));
            ShellRouting.RegisterRouteSafe(nameof(VideoDetailView), typeof(VideoDetailView));
            ShellRouting.RegisterRouteSafe(nameof(VideoForm), typeof(VideoForm));
            ShellRouting.RegisterRouteSafe(nameof(VideoView), typeof(VideoView));

            ShellRouting.RegisterRouteSafe(nameof(OtherForm), typeof(OtherForm));
            ShellRouting.RegisterRouteSafe(nameof(OtherView), typeof(OtherView));
            ShellRouting.RegisterRouteSafe(nameof(OtherDetailView), typeof(OtherDetailView));

            ShellRouting.RegisterRouteSafe(nameof(MusicForm), typeof(MusicForm));
            ShellRouting.RegisterRouteSafe(nameof(MusicView), typeof(MusicView));
            ShellRouting.RegisterRouteSafe(nameof(MusicDetailView), typeof(MusicDetailView));


            ShellRouting.RegisterRouteSafe(nameof(BookForm), typeof(BookForm));
            ShellRouting.RegisterRouteSafe(nameof(BookView), typeof(BookView));
            ShellRouting.RegisterRouteSafe(nameof(BookDetailView), typeof(BookDetailView));*/

            // Register named routes for BookHome and Home so they can be navigated to by route name
            Routing.RegisterRoute("bookhome", typeof(BookHome));
            Routing.RegisterRoute("home", typeof(MainPage));
            Routing.RegisterRoute(nameof(BookForm), typeof(BookForm));
            // Ensure BookForm route is registered
            ShellRouting.RegisterRouteSafe(nameof(BookForm), typeof(BookForm));
        }
        
        private async void OnImportDataClicked(object sender, EventArgs e)
        {
            string path = Path.GetFullPath("downloads");
            try
            {
                var message = $"Import succeeded";
                var result = await FilePicker.PickAsync(default);
                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using (var workbook = new ClosedXML.Excel.XLWorkbook(stream))
                    {
                        bool importResult = await ExcelController.ImportExcelData(workbook);
                        if (!importResult) {
                            message = $"Import failed";
                        }
                    }
                }
                

                await Shell.Current.DisplayAlert("Import", message ?? "", "OK");
            }
            catch (Exception ex)
            {
                var message = ex is OperationCanceledException
                    ? "Import canceled"
                    : $"Failed to import the data";

                await Shell.Current.DisplayAlert("Import", message ?? "", "OK");
            }
        }
        /*
        private async void OnExportDataClicked(object sender, EventArgs e)
        {
            string path = Path.GetFullPath("downloads");
            try
            {
                var result = await FolderPicker.PickAsync(default);
                if (result != null)
                {
                    path = result.Folder.Path;
                }

                var message = $"Import succeeded";

                //SQLExport.Export(_dbContext, path);

                await Shell.Current.DisplayAlert("Export", message ?? "", "OK");
            }
            catch (Exception ex)
            {
                var message = ex is OperationCanceledException
                   ? "Export canceled"
                   : $"Failed to export the data";

                await Shell.Current.DisplayAlert("Export", message ?? "", "OK");
            }
        }
        */

        private async void OnMenuBooksClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//bookhome");
        }

        private async void OnMenuHomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//home");
        }
    }

    static class ShellRouting
    {
        static readonly HashSet<string> _registeredRoutes = new();

        public static void RegisterRouteSafe(string route, Type pageType)
        {
            if (string.IsNullOrWhiteSpace(route) || pageType is null) return;
            if (_registeredRoutes.Add(route))
                Routing.RegisterRoute(route, pageType);
        }
    }
}

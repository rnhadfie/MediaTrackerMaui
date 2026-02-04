using MauiApp1.Service.Modals;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Controllers.ViewModels
{
    public class AddBookViewModel
    {
        public bool AddNewSeries { get; set; }
        public Book Book { get; set; }
    }
}

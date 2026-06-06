using MauiApp1.BackEnd.Models;
using SQLite;
using System;
using System.Collections.Generic;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.Front.Components.Books.ViewModels
{
    public class BookFormViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public string? Artist { get; set; }

        public int? Publisher { get; set; }
        public int[] Genre { get; set; }
        public int Format { get; set; }
        public string Volume { get; set; }
        public bool Read { get; set; }
        public Language Language { get; set; }

        // Selected items from multi-picker (stores selected option objects)
        public List<TextValuePair<string, int>> SelectedGenres { get; set; }

        public int BookSeries { get; set; }

        public bool IsSeriesSelected { get;set;}
        public BookType? Type { get; set; }


        public string NewBookSeriesName { get; set; }
        public List<TextValuePair<string, int>> BookSeriesList { get; set; }
        public List<TextValuePair<string, int>> Publishers { get; set; }

        public List<TextValuePair<string, int>> GenreOptions { get; set; }

    }
}

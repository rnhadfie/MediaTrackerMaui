using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MauiApp1.Service.Modals
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Title { get; set; }
        public byte[]? Cover { get; set; }
        public string? Author {  get; set; }
        public string? Artist { get; set; }
        
        public string? Publisher { get; set; }
        public int Genre { get; set; }
        public int Format { get; set; }
        public int Type { get; set; }
        public int Volume { get; set; }

        [ForeignKey("Series")]
        public int SeriesId { get; set; }

    }
}

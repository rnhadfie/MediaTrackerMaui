using MauiApp1.BackEnd.Service.Modals;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Service.Modals
{
    public class Video
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Name { get; set; }

        public int? Category { get; set; }

        [ForeignKey("Series")]
        public int Series { get; set; }

        public int format { get; set; }

        public int Type { get; set; }

        public byte[]? Cover { get; set; }

        public bool IsLimitedEdition { get; set; }

    }
}

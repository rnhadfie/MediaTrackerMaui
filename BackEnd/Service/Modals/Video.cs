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

        public string Name { get; set; }

        public int? Category { get; set; }

        public int Series { get; set; }

        public VideoFormat format { get; set; }

        public VideoType Type { get; set; }

        public byte[]? Cover { get; set; }

        public bool IsLimitedEdition { get; set; }

    }
}

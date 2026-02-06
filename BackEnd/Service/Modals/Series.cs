
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.Service.Modals
{
    public class Series
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int SeriesId { get; set; }
        

        public required string Title { get; set; }


        public string? Author { get; set; }
        public string? Artist { get; set; }

        public string? Publisher { get; set; }

        public int? TotalVolumes { get; set; }

        public MediaDataType type { get; set; }

        public CollectionStatus? CollectionStatus { get; set; }
    }
}

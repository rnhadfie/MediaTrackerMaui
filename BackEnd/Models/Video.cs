
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiApp1.Modals
{
    public class Video
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Name { get; set; }

        public int? Category { get; set; }

        [ForeignKey("Series")]
        public int SeriesId { get; set; }

        public int VideoFormat { get; set; }

        public int Type { get; set; }

        public byte[]? Cover { get; set; }
        public int? Genre { get; set; }

    }
}

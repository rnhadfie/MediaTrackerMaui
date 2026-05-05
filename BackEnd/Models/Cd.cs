using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MauiApp1.Modals
{
    public class Cd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }

        public string Artist { get; set; }

        public byte[]? Cover { get; set; }

        public int? Language { get; set; }
        public int? MusicGenre { get; set; }

        [ForeignKey("Series")]
        public int Collection { get; set; }
    }
}

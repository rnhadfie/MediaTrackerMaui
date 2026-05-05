using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MauiApp1.BackEnd.Modals
{
    public class Other
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }

        public byte[]? Image { get; set; }

        [ForeignKey("Series")]
        public int Collection { get; set; }
    }
}

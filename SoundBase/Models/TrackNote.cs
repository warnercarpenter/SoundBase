using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class TrackNote
    {
        [Key]
        public int TrackNoteId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DatePosted { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int TrackId { get; set; }

        public Track Track { get; set; }
        public int? Millisecond { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
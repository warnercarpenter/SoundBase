using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class Track
    {
        [Key]
        public int TrackId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateUploaded { get; set; }

        [Required]
        [StringLength(55)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; }


        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Please select a track")]
        [DisplayName("Upload .wav or .mp3 file")]
        public string FilePath { get; set; }
    }
}

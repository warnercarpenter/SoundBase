using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class ProjectUser
    {
        [Key]
        public int ProjectUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateAdded { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; }

        [Required]
        public bool IsAdmin { get; set; }
    }
}

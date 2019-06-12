using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        [Required]
        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }

        [Required]
        [StringLength(55)]
        public string ArtistName { get; set; }

        [Required]
        [StringLength(55)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [DisplayName("Upload Image")]
        public string ImagePath { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
        public virtual ICollection<ProjectInvite> ProjectInvites { get; set; }
    }
}

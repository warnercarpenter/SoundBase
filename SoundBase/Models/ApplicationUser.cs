using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SoundBase.Models
{
    /*
        Add profile data for application users by adding
        properties to the ApplicationUser class
    */
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public virtual ICollection<TrackNote> TrackNotes { get; set; }

        public virtual ICollection<ChatMessage> ChatMessages { get; set; }

        public virtual ICollection<Project> ProjectsCreated { get; set; }

        [InverseProperty("Sender")]
        public virtual ICollection<ProjectInvite> ProjectInvitesSent { get; set; }

        [InverseProperty("Receiver")]
        public virtual ICollection<ProjectInvite> ProjectInvitesReceived { get; set; }

        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
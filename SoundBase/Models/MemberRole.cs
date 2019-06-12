using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class MemberRole
    {
        [Key]
        public int MemberRoleId { get; set; }

        [Required]
        [StringLength(55)]
        public string Title { get; set; }

        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }
}

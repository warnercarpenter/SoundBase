using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class InviteUserViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }
    }
}

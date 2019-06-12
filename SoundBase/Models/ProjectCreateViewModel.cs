using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class ProjectCreateViewModel
    {
        public Project Project { get; set; }

        public IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "Please select your role")]
        [Display(Name = "Your role")]
        public int MemberRoleId { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class ProjectChatViewModel
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Required]
        public string ChatMessage { get; set; }
    }
}
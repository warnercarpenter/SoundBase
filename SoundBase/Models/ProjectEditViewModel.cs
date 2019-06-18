using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class ProjectEditViewModel
    {
        public Project Project { get; set; }

        public IFormFile ImageFile { get; set; }

        public string ImagePath { get; set; }
    }
}
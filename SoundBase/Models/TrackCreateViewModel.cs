using System;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class TrackCreateViewModel
    {
        public Track Track { get; set; }

        public IFormFile AudioFile { get; set; }
    }
}

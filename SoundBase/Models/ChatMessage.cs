using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoundBase.Models
{
    public class ChatMessage
    {
        [Key]
        public int ChatMessageId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DatePosted { get; set; }

        [Required(ErrorMessage = "Please enter a message")]
        public string Content { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}

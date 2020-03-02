using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUTrivBot.Models
{
    public class GuildSettings
    {
        [Key]
        public string GuildId { get; set; }
        
        [Required]
        public bool Disabled { get; set; }
        
        [Required]
        public bool RestrictTrivMaster { get; set; }
        
        [Required]
        public List<Channel> LockedChannels { get; } = new List<Channel>();
    }
}
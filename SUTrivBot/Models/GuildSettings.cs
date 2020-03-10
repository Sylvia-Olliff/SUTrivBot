using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUTrivBot.Models
{
    public class GuildSettings
    {
        public ulong GuildId { get; set; }
        
        public string GuildName { get; set; }
        
        public bool Disabled { get; set; }
        
        public bool RestrictTrivMaster { get; set; }
        
        public List<Channel> LockedChannels { get; } = new List<Channel>();
    }
}
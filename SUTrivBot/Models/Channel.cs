
using System.ComponentModel.DataAnnotations;

namespace SUTrivBot.Models
{
    public class Channel
    {
        [Key]
        public int ChannelId { get; set; }
        public string GuildId { get; set; }
        
        public GuildSettings GuildSet { get; set; }
        
        public string ChannelName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Channel))
                return false;

            var otherChannel = (Channel) obj;

            return otherChannel.ChannelName == ChannelName && otherChannel.GuildId == GuildId;
        }
    }
}
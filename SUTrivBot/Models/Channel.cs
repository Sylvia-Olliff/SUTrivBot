
namespace SUTrivBot.Models
{
    public class Channel
    {
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
        
        public GuildSettings GuildSet { get; set; }
        
        public string ChannelName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Channel))
                return false;

            var otherChannel = (Channel) obj;

            return otherChannel.ChannelId == ChannelId && otherChannel.GuildId == GuildId;
        }
    }
}
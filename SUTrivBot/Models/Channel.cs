
namespace SUTrivBot.Models
{
    public class Channel
    {
        public string GuildId { get; set; }
        
        public GuildSettings GuildSet { get; set; }
        
        public string ChannelName { get; set; }
    }
}
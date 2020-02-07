using System.ComponentModel;
using DSharpPlus.Entities;

namespace SUTrivBot.Models
{
    /// <summary>
    /// Immmutable Struct. This is best practice when using an Object as a collection Key
    /// </summary>
    [ImmutableObject(true)]
    public struct GameId
    {
        public DiscordChannel Channel { get; }
        public DiscordGuild Guild { get; }
        public DiscordUser User { get; }
        
        public GameId(DiscordGuild guild, DiscordChannel channel, DiscordUser user)
        {
            Channel = channel;
            Guild = guild;
            User = user;
        }
        
        /// <summary>
        /// Overrides the ToString() method to generate our string key
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Guild.Name}/{Channel.Name}";
        }

        /// <summary>
        /// Dictionaries and other collections when determining if a key exists and the key is an object
        /// use this method for determining equality. This overrides that method to compare the string version
        /// established above. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is GameId))
                return false;

            var otherId = (GameId) obj;

            return otherId.Channel.Id == Channel.Id && otherId.Guild.Id == Guild.Id;
        }
    }
}
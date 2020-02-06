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
        
        public GameId(DiscordGuild guild, DiscordChannel channel)
        {
            Channel = channel;
            Guild = guild;
        }
        
        /// <summary>
        /// Overrides the ToString() method to generate our string key
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Guild.Id}/{Channel.Id}";
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

            return otherId.ToString() == this.ToString();
        }
    }
}
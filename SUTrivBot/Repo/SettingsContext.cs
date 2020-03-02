using Microsoft.EntityFrameworkCore;
using SUTrivBot.Models;

namespace SUTrivBot.Repo
{
    public class SettingsContext : DbContext
    {
        public DbSet<GuildSettings> Guilds { get; set; }
        public DbSet<Channel> Channels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=peribot.db");
    }
}
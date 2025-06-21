using Microsoft.EntityFrameworkCore;
using MoodMapper.Models;

namespace MoodMapper.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<Note> Notes { get; set; }

        public DbSet<EmotionEntry> EmotionEntries { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Statistics> Statistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

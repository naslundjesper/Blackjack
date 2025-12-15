using Microsoft.EntityFrameworkCore;
using Blackjack.Models;

namespace Blackjack.Data
{
    public class BlackjackDbContext : DbContext
    {
        public BlackjackDbContext(DbContextOptions<BlackjackDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<PlayerHand> PlayerHands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Player -> Game (Player1)
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Player1)
                .WithMany(p => p.GamesAsPlayer1)
                .HasForeignKey(g => g.Player1ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Player -> Game (Player2)
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Player2)
                .WithMany(p => p.GamesAsPlayer2)
                .HasForeignKey(g => g.Player2ID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

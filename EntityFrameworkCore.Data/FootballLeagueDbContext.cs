using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Data
{
    public class FootballLeagueDbContext : DbContext
    {
        public FootballLeagueDbContext(DbContextOptions<FootballLeagueDbContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<TeamsAndLeaguesView> TeamsAndLeaguesView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure the view
            modelBuilder.Entity<TeamsAndLeaguesView>()
                .HasNoKey()
                .ToView("vw_TeamsAndLeagues");

            // Configure Team-Match relationships
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Define DB function
            modelBuilder.HasDbFunction(typeof(FootballLeagueDbContext)
                .GetMethod(nameof(GetEarliestTeamMatch), new[] { typeof(int) }))
                .HasName("fn_GetEarliestMatch");
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>()
                .HaveMaxLength(100);
            configurationBuilder.Properties<decimal>()
                .HavePrecision(16, 2);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries<BaseDomainModel>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.ModifiedDate = DateTime.UtcNow;
                entry.Entity.ModifiedBy = "Sample User 1";

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "Sample User";
                }

                entry.Entity.Version = Guid.NewGuid();
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public DateTime GetEarliestTeamMatch(int teamId) => throw new NotImplementedException();
    }
}
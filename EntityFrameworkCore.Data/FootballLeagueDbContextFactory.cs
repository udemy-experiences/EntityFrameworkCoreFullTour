using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EntityFrameworkCore.Data
{
    public class FootballLeagueDbContextFactory : IDesignTimeDbContextFactory<FootballLeagueDbContext>
    {
        public FootballLeagueDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FootballLeagueDbContext>();
            optionsBuilder.UseSqlite(config.GetConnectionString("SqliteDatabaseConnectionString"));

            return new FootballLeagueDbContext(optionsBuilder.Options);
        }
    }
}
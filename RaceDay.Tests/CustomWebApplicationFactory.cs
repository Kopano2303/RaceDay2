
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RaceDay.Data;

namespace RaceDay.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public const string TestJwtKey =
            "RaceDayIntegrationTestsSecretKey1234567890";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] = TestJwtKey,
                        ["ConnectionStrings:RaceDayConnection"] =
                            "Server=(localdb)\\ProjectModels;Database=RaceDayTestDB;Trusted_Connection=True;"
                    });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<RaceDayContext>>();
                services.RemoveAll<RaceDayContext>();

                services.AddDbContext<RaceDayContext>(options =>
                {
                    options.UseInMemoryDatabase("RaceDayIntegrationTests");
                });
            });
        }
    }
}

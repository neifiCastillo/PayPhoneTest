using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeyphoneTest.Models;
using System.Linq;
using Microsoft.AspNetCore.TestHost;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace PeyphoneTest.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Reemplazar autenticación por autenticación falsa
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestAuth";
                    options.DefaultChallengeScheme = "TestAuth";
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", options => { });

                // Usar In-Memory Database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DbpayphoneTestContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DbpayphoneTestContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });


                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DbpayphoneTestContext>();
                db.Database.EnsureCreated();
            });
        }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestAuth");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

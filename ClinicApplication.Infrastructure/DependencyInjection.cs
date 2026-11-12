using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClinicApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string not found.");
            }

            services.AddDbContext<ClinicDbContext>(options =>
            {
                options.UseSqlServer(connectionString);

                options.LogTo(
                    Console.WriteLine,
                    LogLevel.Information);

                options.EnableSensitiveDataLogging();
            });

            return services;
        }
    }
}
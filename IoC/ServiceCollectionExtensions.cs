using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.Sample.Consumers;
using Redis.Sample.Options;

namespace Redis.Sample.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IConfiguration AddOptionsPattern(this IServiceCollection services, HostBuilderContext context)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var path = context.HostingEnvironment.ContentRootPath;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .Build();

            services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
            services.Configure<EmailConsumer.Options>(configuration.GetSection("Consumers:EmailConsumer"));

            return configuration;
        }
    }
}
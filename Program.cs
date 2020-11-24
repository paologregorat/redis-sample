using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.Sample.Abstract;
using Redis.Sample.Adapters;
using Redis.Sample.Consumers;
using Redis.Sample.IoC;

namespace Redis.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptionsPattern(hostContext);
                    services.AddSingleton<IAdapter, RedisAdapter>();

                    services.AddHostedService<EmailConsumer>();
                    // services.AddHostedService<Producer>();
                });
    }
}
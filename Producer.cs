using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redis.Sample.Model;
using StackExchange.Redis;

namespace Redis.Sample
{
    public class Producer : BackgroundService
    {
        private readonly ILogger<Producer> _logger;

        public Producer(ILogger<Producer> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var multiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");

            var db = multiplexer.GetDatabase();

            var i = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var email = new Email
                {
                    Subject = $"Henshin {i++}"
                };

                var value = new RedisValue(JsonSerializer.Serialize(email));

                Console.WriteLine($"PRODUCED: {JsonSerializer.Serialize(email)}");

                await db.ListRightPushAsync(new RedisKey("email_messages"), value, When.Always, CommandFlags.FireAndForget);

                // await Task.Delay(millisecondsDelay: 500, cancellationToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Redis.Sample.Abstract;
using Redis.Sample.Model;

namespace Redis.Sample.Consumers
{
    public class EmailConsumer : BackgroundService
    {
        private readonly IAdapter _adapter;
        private readonly ILogger<EmailConsumer> _logger;
        private readonly Options _options;

        public class Options
        {
            public string? Key { get; set; }
            public int? Delay { get; set; }
        }

        public EmailConsumer(IAdapter adapter, IOptions<Options> options, ILogger<EmailConsumer> logger)
        {
            _adapter = adapter;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            static void Selector(Email email)
            {
                Console.WriteLine(JsonSerializer.Serialize(email));
            }

            _logger.LogInformation("EmailConsumer is starting.");

            await _adapter.StartConsumeAsync<Email>(_options.Key ?? throw new NullReferenceException("Key cannot be null"), Selector, _options.Delay ?? 2000, cancellationToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailConsumer is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
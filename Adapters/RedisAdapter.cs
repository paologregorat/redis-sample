using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Redis.Sample.Abstract;
using Redis.Sample.Extensions;
using Redis.Sample.Options;
using StackExchange.Redis;

namespace Redis.Sample.Adapters
{
    public class RedisAdapter : IAdapter
    {
        private readonly string? _connectionString;
        private readonly ILogger<RedisAdapter> _logger;

        public RedisAdapter(IOptions<ConnectionStrings> options, ILogger<RedisAdapter> logger)
        {
            _connectionString = options.Value.Redis;
            _logger = logger;
        }

        public async Task StartConsumeAsync<T>(string key, Action<T> selector, int delay = 2000, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Start consuming messages with key: {key}!");

            while (!cancellationToken.IsCancellationRequested)
            {
                await ConsumeAsync(key, selector);
                await Task.Delay(delay, cancellationToken);
            }

            _logger.LogInformation($"Stop consuming messages with key: {key}!");
        }

        private async Task ConsumeAsync<T>(RedisKey key, Action<T> selector)
        {
            var multiplexer = await ConnectionMultiplexer.ConnectAsync(_connectionString);
            var database = multiplexer.GetDatabase();

            var messages = (await database.GetChunk(key, start: 0, stop: 99)).ToImmutableList();

            while (messages.Any())
            {
                foreach (var message in messages)
                {
                    if (!message.HasValue) continue;

                    var parsed = JsonSerializer.Deserialize<T>(message.ToString());

                    if (parsed == null)
                        _logger.LogError($"Cannot parse message. {message}");
                    else
                        selector(parsed);

                    await database.ListRemoveAsync(key, message);
                }

                messages = (await database.GetChunk(key, start: 0, stop: 99)).ToImmutableList();
            }
        }

        private static async Task<List<RedisValue>> GetChunk<T>(RedisKey key, IDatabaseAsync database)
        {
            var messages = (await database.ListRangeAsync(key, start: 0, stop: 99)).ToList();
            return messages;
        }
    }
}
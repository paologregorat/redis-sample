using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Sample.Extensions
{
    public static class RedisExtensions
    {
        public static async Task<IEnumerable<RedisValue>> GetChunk(this IDatabaseAsync database, RedisKey key, int start = 0, int stop = 1) => await database.ListRangeAsync(key, start, stop);
    }
}
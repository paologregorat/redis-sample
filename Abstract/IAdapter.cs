using System;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Sample.Abstract
{
    public interface IAdapter
    {
        Task StartConsumeAsync<T>(string key, Action<T> selector, int delay = 2000, CancellationToken cancellationToken = default);
    }
}
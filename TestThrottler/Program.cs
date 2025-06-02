using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ccxt;

class Program
{
    static async Task Main(string[] args)
    {
        var exchange = new ccxt.bitget(new Dictionary<string, object>
        {
            { "enableRateLimit", true },
            { "rateLimiterAlgorithm", "rollingWindow" },
            { "maxLimiterRequests", 4000 },
        });

        var startTime = Stopwatch.StartNew();
        var tasks = new List<Task>();

        for (int i = 0; i < 60; i++)
        {
            int index = i;
            tasks.Add(Task.Run(async () =>
            {
                var result = await exchange.FetchTicker("BTC/USDT");
                var elapsed = startTime.ElapsedMilliseconds;
                Console.WriteLine($"Call {index + 1} completed in {elapsed} ms");
            }));
        }

        await Task.WhenAll(tasks);
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ccxt;

class Program
{
    static async Task Main(string[] args)
    {
        var exchange = new ccxt.binance(new Dictionary<string, object>
        {
            { "enableRateLimit", true },
            { "rateLimiterAlgorithm", "rollingWindow" },
            { "maxLimiterRequests", 5000 },
        });

        using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(120)); // 120 second total timeout

        try
        {
            // First load the markets
            Console.WriteLine("Loading markets...");
            await exchange.LoadMarkets();
            Console.WriteLine("Markets loaded successfully");

            var startTime = Stopwatch.StartNew();
            var tasks = new List<Task>();
            var numRequests = 5000;

            for (int i = 0; i < numRequests; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        // Use per-request timeout of 5 seconds
                        var result = await WithTimeout(exchange.FetchTicker("BTC/USDT"), 120000);
                        var elapsed = startTime.ElapsedMilliseconds;
                        Console.WriteLine($"Call {index + 1} completed in {elapsed} ms");
                    }
                    catch (TimeoutException)
                    {
                        var elapsed = startTime.ElapsedMilliseconds;
                        Console.WriteLine($"Call {index + 1} failed: Timeout after 5000 ms (elapsed {elapsed} ms)");
                    }
                    catch (Exception ex)
                    {
                        var elapsed = startTime.ElapsedMilliseconds;
                        Console.WriteLine($"Call {index + 1} failed: {ex.ToString()} (elapsed {elapsed} ms)");
                    }
                }, cts.Token));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"All tasks completed in {startTime.ElapsedMilliseconds} ms");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation timed out after 30 seconds");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }

    // Helper function: run a task with a timeout in milliseconds
    static async Task<T> WithTimeout<T>(Task<T> task, int timeoutMs)
    {
        if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
        {
            // Completed within timeout
            return await task;
        }
        else
        {
            throw new TimeoutException("Operation timed out");
        }
    }
}

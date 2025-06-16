import asyncio
import time
import sys
sys.path.insert(0, '/Users/caoilainn/Documents/ccxt/python')
import ccxt.async_support as ccxt


async def fetch_tickers_parallel(times, symbol):
    exchange = ccxt.binance({
        'enableRateLimit': True,
        # 'rateLimiterAlgorithm': 'leakyBucket',
        'rateLimiterAlgorithm': 'rollingWindow',
        'maxLimiterRequests': 1000,
    })
    try:
        start_time = time.time()
        tasks = []
        for i in range(times):
            tasks.append(fetch_ticker(exchange, symbol, i, start_time))
        await asyncio.gather(*tasks)
    finally:
        await exchange.close()


async def fetch_ticker(exchange, symbol, index, start_time):
    try:
        result = await exchange.fetch_ticker(symbol)
        elapsed = (time.time() - start_time) * 1000
        print(f"Call {index + 1} completed in {elapsed:.2f} ms")
        return result
    except Exception as e:
        print(f"Call {index + 1} failed: {e}")


num_requests = 5000
asyncio.run(fetch_tickers_parallel(num_requests, 'BTC/USDT'))

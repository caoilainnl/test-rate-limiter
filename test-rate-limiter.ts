import ccxt from "ccxt";

const exchange = new ccxt.binance ({
    enableRateLimit: true,
    // rateLimiterAlgorithm: "leakyBucket",
    rateLimiterAlgorithm: "rollingWindow",
    maxLimiterRequests: 5000,
    timeout: 90000,
});

async function fetchTickersParallel(times: number, symbol: string): Promise<void> {
    const startTime = Date.now();
    const requests = [...Array(times)].map(async (_, index) => {
        try {
            const result = await exchange.fetchTicker("BTC/USDT");
            console.log(`Call ${index + 1} completed in ${Date.now() - startTime} ms`);
            return result;
        } catch (error) {
            console.error(`Call ${index + 1} failed: ${error}`);
        }
    });
    await Promise.all(requests);
}

const numRequests = 1000;
fetchTickersParallel(numRequests, "BTC/USDT").catch(console.error);
import ccxt from "ccxt";

const exchange = new ccxt.bitget ({
    enableRateLimit: true,
    rateLimiterAlgorithm: "rollingWindow",
    maxLimiterRequests: 4000,
});

async function fetchTickersParallel(times: number, symbol: string): Promise<void> {
    const startTime = Date.now();
    const requests = [...Array(times)].map(async (_, index) => {
        const result = await exchange.fetchTicker("BTC/USDT");
        console.log(`Call ${index + 1} completed in ${Date.now() - startTime} ms`);
        return result;
    });
}

fetchTickersParallel(60, "BTC/USDT").catch(console.error);
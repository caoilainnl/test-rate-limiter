package main

import (
    "fmt"
    "sync"
    "time"

    "github.com/ccxt/ccxt/go/v4"
)

func main() {
    exchange := ccxt.NewBitget(map[string]interface{}{
        "enableRateLimit":     true,
        "rateLimiterAlgorithm": "rollingWindow",
        "maxLimiterRequests":  4000,
    })

    var wg sync.WaitGroup
    startTime := time.Now()

    for i := 0; i < 60; i++ {
        wg.Add(1)
        go func(index int) {
            defer wg.Done()
            result, err := exchange.FetchTicker("BTC/USDT")
            if err != nil {
                fmt.Printf("Error fetching ticker: %v\n", err)
                return
            }
            elapsed := time.Since(startTime).Milliseconds()
            fmt.Printf("Call %d completed in %d ms\n", index+1, elapsed)
        }(i)
    }

    wg.Wait()
}

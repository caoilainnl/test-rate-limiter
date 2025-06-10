package main

import (
	"fmt"
	"sync"
	"time"

	ccxt "github.com/ccxt/ccxt/go/v4"
)

func main() {
	exchange := ccxt.NewBinance(map[string]interface{}{
		"enableRateLimit": true,
		// "rateLimiterAlgorithm": "leakyBucket",
		"rateLimiterAlgorithm": "rollingWindow",
		"maxLimiterRequests":   5000,
	})

	<-exchange.LoadMarkets()
	var wg sync.WaitGroup
	startTime := time.Now()

	numRequests := 5000

	for i := 0; i < numRequests; i++ {
		wg.Add(1)
		go func(index int) {
			defer wg.Done()
			_, err := exchange.FetchTicker("BTC/USDT")
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

<?php
ini_set('memory_limit', '2G');
require 'vendor/autoload.php';

use React\EventLoop\Factory;
use React\Promise\PromiseInterface;

$loop = Factory::create();

if ($loop instanceof React\EventLoop\StreamSelectLoop && extension_loaded('event')) {
    $loop = new ExtEventLoop();
    echo "Switched to ExtEventLoop\n";
}

$exchange_class = '\\ccxt\\async\\binance';
$exchange = new $exchange_class([
    'enableRateLimit' => true,
    // 'rateLimiterAlgorithm' => 'leakyBucket',
    'rateLimiterAlgorithm' => 'rollingWindow',
    'maxLimiterRequests' => 5000,
]);

$start_time = microtime(true);
$promises = [];
$num_requests = 5000;

for ($i = 0; $i < $num_requests; $i++) {
    $promises[] = (function ($i) use ($exchange, $start_time): PromiseInterface {
        try {
            return $exchange->fetch_ticker('BTC/USDT')->then(
                function ($result) use ($i, $start_time) {
                    $elapsed = (microtime(true) - $start_time) * 1000;
                    echo "Call " . ($i + 1) . " completed in " . round($elapsed, 2) . " ms\n";
                }
            )->otherwise(
                function ($error) use ($i, $start_time) {
                    $elapsed = (microtime(true) - $start_time) * 1000;
                    echo "Call " . ($i + 1) . " failed in " . round($elapsed, 2) . " ms: " . $error->getMessage() . "\n";
                }
            );
        } catch (Exception $e) {
            echo "Immediate exception in call " . ($i + 1) . ": " . $e->getMessage() . "\n";
            return \React\Promise\resolve(null);
        }
    })($i);
}

\React\Promise\all($promises)->then(function () use ($loop) {
    $loop->stop();
});

$loop->run();
?>

<?php
require 'vendor/autoload.php';

use React\EventLoop\Factory;

$loop = Factory::create();

$exchange_class = '\\ccxt\\async\\bitget';
$exchange = new $exchange_class([
    'enableRateLimit' => true,
    'rateLimiterAlgorithm' => 'rollingWindow',
    'maxLimiterRequests' => 4000,
]);

$start_time = microtime(true);
$promises = [];

for ($i = 0; $i < 60; $i++) {
    $promises[] = $exchange->fetch_ticker('BTC/USDT')->then(function ($result) use ($i, $start_time) {
        $elapsed = (microtime(true) - $start_time) * 1000;
        echo "Call " . ($i + 1) . " completed in " . round($elapsed, 2) . " ms\n";
        return $result;
    });
}

\React\Promise\all($promises)->then(function () use ($loop) {
    $loop->stop();
});

$loop->run();

?>
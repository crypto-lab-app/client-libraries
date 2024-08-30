# Libraries Python for CryptoLab
Website: https://www.crypto-lab.io  
Documentation: https://www.crypto-lab.io/documentation  
Swagger: https://www.crypto-lab.io/swagger

## .NET
Available on Microsoft Nuget: https://www.nuget.org/packages/CryptoLab/
Sample to use it to replay data 
```csharp
using CryptoLab;

// Show the version lib
Console.WriteLine("Verison: " + CryptoLab.CryptoLabAPI.version().ToString());

// Init the client
CryptoLabAPI client = new CryptoLabAPI("YOUR_APPI_KEY", true);

// Get list of exchanges available and displayt it
List<Exchange> exchanges = client.get_exchanges();
foreach(Exchange exchange in exchanges)
    Console.WriteLine(exchange.exchange);

// Get list of markets available for binance
List<Market> markets = client.get_markets(new Exchange { exchange = "binance" });
Console.WriteLine(markets.Count + " markets available for Binance");
foreach (Market market in markets)
    Console.WriteLine("Market " + market.market + ": data available from " + market.first_record + " to " + market.last_record + ". Total size " + ConvertBytes(market.bytes));

// Init the replay data for
client.init_replay(callback, new Exchange { exchange = "binance" }, new Market { market = "btc_usdt" }, "2022-05-07", "2022-06-07", false);
client.start_replay();
Console.WriteLine("Strating replay for Binance btc_usdt (downloading data in " + client.get_cache_directory() + ". Could be long)");

// Callback on event (trade and orderbook)
void  callback(Trade trade)
{
    /* Add your trading algorithm here */

    if(trade != null) // If you stop (not definitivly) the replay, the callback function is called but the trade is null
        Console.WriteLine("Trade id: " + trade.trade_id);

    // Show error (nothing if no error)
    foreach (string error in client.get_errors())
        Console.WriteLine("Error: " + error);
}
```

Output:
![Logo](https://www.crypto-lab.io/img/resultat_lib.png)


![Logo](https://1.gravatar.com/avatar/5121577298f39a1661507198f8615319a7d7a14fad36f9ec52d20ae0d446bf69?size=128)

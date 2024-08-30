# Libraries for CryptoLab
Website: https://www.crypto-lab.io  
Documentation: https://www.crypto-lab.io/documentation  
Swagger: https://www.crypto-lab.io/swagger
Git: https://github.com/crypto-lab-io/client-libraries

Libraries for:
* Python3
* .NET
* PHP (soon)

## Python

Available on PyPi: https://pypi.org/project/Cryptolab/
Install library from Pypi ```pip install cryptolab```

Sample to use it to replay data 
```python
import cryptolab

# Init lib with api key
cl = cryptolab.CryptoLab('{YOUR_API_KEY}', on_error)

# Init the raplayer with the parameters
cl.init_replayer(event, '{EXCHANGE}', '{MARKET}', '{START_DATE}', '{END_DATE}')

# On event - callback
def event(self, trade, message=None):

    if(message):
        print(message)

    if(trade):
        print(trade)
    # add you algorithm here to backtest your strategy

# On event error
def on_error(message):
    print(message) # ex: quota reached, data not avaible, plan inactive, etc.
```

## .NET
Available on Microsoft Nuget: https://www.nuget.org/packages/CryptoLab/

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

// Callback on event (message or trade)
void  callback(object data)
{
    /* Add your trading algorithm here */


    // If you stop (not definitivly) the replay, the callback function is called but the trade is null
    if (data == null)
        return;

    // If data is a message
    if (data is string)
        Console.WriteLine("Message: " + data);

    // If data is trade
    if(data is Trade) {
        Trade? trade = data as Trade;
        Console.WriteLine("Trade id: " + trade.trade_id);
    }
}
```

Output:
![Logo](https://raw.githubusercontent.com/crypto-lab-io/client-libraries/main/Other/result_lib.png)

## PHP
soon

![Logo](https://1.gravatar.com/avatar/5121577298f39a1661507198f8615319a7d7a14fad36f9ec52d20ae0d446bf69?size=128)
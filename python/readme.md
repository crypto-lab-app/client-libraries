# Libraries Python for CryptoLab
Website: https://www.crypto-lab.app   
Documentation: https://www.crypto-lab.app/documentation  
Swagger: https://www.crypto-lab.app/swagger  

## Python
Install library from Pypi ```pip install cryptolab```

Sample to use it to replay data 
```python
from cryptolab import CryptoLab

# Init lib with api key
cl = CryptoLab('{YOUR_API_KEY}', on_error)

# Init the raplayer with the parameters
cl.init_replayer(on_event, '{EXCHANGE}', '{MARKET}', '{START_DATE}', '{END_DATE}')

# Replay  On event - callback
def on_event(self, trade, message=None):

    if(message):
        print(message)

    if(trade):
        print(trade)
    # add you algorithm here to backtest your strategy

# On event error
def on_error(message):
    print(message) # ex: quota reached, data not avaible, plan inactive, etc.
```


![Logo](https://1.gravatar.com/avatar/5121577298f39a1661507198f8615319a7d7a14fad36f9ec52d20ae0d446bf69?size=128)

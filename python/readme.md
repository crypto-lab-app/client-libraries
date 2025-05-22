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

![Logo](https://0.gravatar.com/avatar/bfc7517f0c195bb6ca79c763a7163880638282c445980bd77ac5f61cead1e5ae?size=128)

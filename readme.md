# Libraries for CryptoLab
Website: https://www.crypto-lab.io  
Documentation: https://www.crypto-lab.io/documentation  
Swagger: https://www.crypto-lab.io/swagger

Libraries for:
* Python3


## Python
Download python lib from this repository. Then:
```python
import maketlab

# Init lib with api key
cl = maketlab.CryptoLab('{YOUR_API_KEY}', on_error)

# Init the raplayer with the parameters
cl.init_replayer(event, '{EXCHANGE}', '{MARKET}', '{START_DATE}', '{END_DATE}')

# On event - callback
def event(trade):
    print(trade)
    # add you algorithm here to backtest your strategy

# On event error
def on_error(message):
    print(message) # quota reached, data not avaible, plan inactive, etc.
```
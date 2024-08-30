import cryptolab

class Sample:
    def __init__(self):
        # Init lib with api key, exhchange and market
        self.cl = cryptolab.CryptoLab('{YOUR_API_KEY}', self.event_error)
        
        # Call samples
        print(self.cl.get_exchanges())
        print(self.cl.get_markets('binance'))
        print(self.cl.get_files('binance', 'eth_btc', '2020-04-06', '2024-08-15'))
        
        
        # Replay sample (for Binance Exchange for market eth_btc from 2020-04-06 to 2024-08-15)
        self.cl.init_replayer(self.event, 'binance', 'eth_btc', '2020-04-06', '2024-08-15')
       
    # Replay  On event - callback
    def event(self, trade, message=None):

        if(message):
            print(message)

        if(trade):
            print(trade)
        
    # On event error
    def event_error(self, msg):
        print('event_error - ' + str(msg))
        
if __name__ == '__main__':
    Sample()
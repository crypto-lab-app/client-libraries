from setuptools import setup
from sys import version_info

setup(name='cryptolab',
      version='0.0.1',
      description='Cryptolab library to replay historic data',
      url='https://www.crypto-lab.io',
      author='CryptoLab, Charles',
      author_email='contact@crypto-lab.io',
      license='MIT',
      packages=['cryptolab'],
      install_requires=['pandas', 'requests'],
      keywords='cryptolab backtest cryptocurrency cryptocurrencies api bitcoin binance gateio'
      )
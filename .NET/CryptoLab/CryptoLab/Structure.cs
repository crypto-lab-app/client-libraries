using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CryptoLab {

    // Class for error
    public class Error {
        public bool success { get; set; }
        public string message { get; set; }
        public object resuts { get; set; }
    }

    // Class for information about market
    public class InformationMarket {
        public string exchange { get; set; }
        public string market { get; set; }
        public DateTime first_record { get; set; }
        public DateTime last_record { get; set; }
        public int total_size { get; set; }
    }

    public class RootObjectInformationMarket {
        public bool success { get; set; }
        public string message { get; set; }
        public InformationMarket results { get; set; }
    }

    // Class for list of exchanges
    public class Exchange
    {
        public string exchange { get; set; }
    }

    public class RootObjectExchanges
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<Exchange> results { get; set; }
    }



    // Class for list of markets
       public class Market
    {
        public string market { get; set; }
        public string first_record { get; set; }
        public string last_record { get; set; }
        public Int64 bytes { get; set; }
    }

    public class RootObjectMarkets
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<Market> results { get; set; }
    }


    // Class for list of files
        public class File
    {
        public string date { get; set; }
        public Int64 bytes { get; set; }
    }

    public class RootObjectFiles
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<File> results { get; set; }
    }

    // Class for trade
    public class Trade {
        public Trade(Dictionary<string, object> trade) {
            if (trade.Count != 8)
                return;
            base_currency = trade["base_currency"].ToString();
            counter_currency = trade["counter_currency"].ToString();
            trade_time = UInt64.Parse(trade["trade_time"].ToString(), CultureInfo.InvariantCulture);
            trade_id = UInt64.Parse(trade["trade_id"].ToString(), CultureInfo.InvariantCulture);
            price = double.Parse(trade["price"].ToString(), CultureInfo.InvariantCulture);
            size = double.Parse(trade["qty"].ToString(), CultureInfo.InvariantCulture);
            is_buyer_maker = bool.Parse(trade["isBuyerMaker"].ToString());
            is_best_match = bool.Parse(trade["isBestMatch"].ToString());
        }

        public string base_currency { get; set; }

        public string counter_currency { get; set; }

        public UInt64 trade_time { get; set; }

        public UInt64 trade_id { get; set; }

        public double price { get; set; }

        public double size { get; set; }

        public bool is_buyer_maker { get; set; }

        public bool is_best_match { get; set; }
    }
}
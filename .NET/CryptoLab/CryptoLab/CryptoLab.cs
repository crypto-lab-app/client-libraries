using RestSharp;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using CsvHelper;
using System.Text.Json;

using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper.Configuration;
using System.Reflection;
using System;
using System.Diagnostics;

namespace CryptoLab
{
    public class CryptoLabAPI
    {

        // Variables globales
        private const string _BASE_API_ = "https://api.crypto-lab.io/";
        private bool show_errors = false;
        private string apiKey = null;
        private RestClient client = null;
        private List<string> errors = new List<string>();

        // Variables globales - replay part
        private Thread thread_replay = null;
        private ShouldTread thread_shouldState = ShouldTread.Run;
        public delegate void CallBack(object data);
        private CallBack callback = null;
        private Exchange replay_exchange = null;
        private Market replay_market = null;
        private string replay_type = null;
        private string replay_start_date = null;
        private string replay_end_date = null;

        private enum ShouldTread
        {
            Run = 1,
            Wait = 2,
            End = 3
        }

        /// <summary>
        /// Get the current version of lib
        /// </summary>
        /// <returns>Version</returns>
        public static Version version()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Initialize the client
        /// </summary>
        /// <param name="apiKey">Your API key (find it in your account on market-lab.app)</param>
        /// <param name="show_errors">Show the error with popup during the replay</param>
        public CryptoLabAPI(string apiKey, bool show_errors = true)
        {
            this.apiKey = apiKey;
            this.show_errors = show_errors;
            client = new RestClient(_BASE_API_);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("X-API-Key", this.apiKey);
        }

        /// <summary>
        /// Add API key if init without
        /// </summary>
        /// <param name="apiKey">Your API key (find it in your account on market-lab.app)</param>
        public void set_api_key(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Get list of exchanges
        /// </summary>
        /// <returns>Object with status of request and the list of exchanges</returns>
        public List<Exchange> get_exchanges()
        {
            try
            {
                string json = this.execute_request("data/exchanges");
                if (json != null)
                    return JsonSerializer.Deserialize<RootObjectExchanges>(json).results;
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("get_exchanges - " + ex.Message);
            }
        }

        /// <summary>
        /// Get list of markets for the exchange
        /// </summary>
        /// <param name="exchange">Name of exchange</param>
        /// <returns></returns>
        public List<Market> get_markets(Exchange exchange)
        {
            try
            {
                string json = this.execute_request("data/" + exchange.exchange + "/markets");
                if (json != null)
                    return JsonSerializer.Deserialize<RootObjectMarkets>(json).results;
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("get_markets - " + ex.Message);
            }
        }

        /// <summary>
        /// Get list of files for the exchange, markets, dates and type
        /// </summary>
        /// <param name="exchange">Name of the exchange</param>
        /// <param name="market">Name of the market</param>
        /// <param name="start_date">Date to start 'YYYY-MM-DD'</param>
        /// <param name="end_date">Date to end 'YYYY-MM-DD'</param>
        /// <returns></returns>
        public List<File> get_files(Exchange exchange, Market market, string start_date, string end_date)
        {
            try
            {
                string json = this.execute_request("data/" + exchange.exchange.ToLower() + "/" + market.market.ToLower() + "/" + start_date + "/" + end_date);
                if (json != null)
                    return JsonSerializer.Deserialize<RootObjectFiles>(json).results;
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("get_files - " + ex.Message);
            }
        }

        // Send a request to the API
        private string execute_request(string url_parameters)
        {
            try
            {
                // Create request
                RestRequest request = new RestRequest(url_parameters, Method.Get);

                // Execute request
                RestResponse response = client.Execute(request);
                // If error
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("Status code: " + response.StatusCode);
                

                return response.Content;
            }
            catch (Exception ex)
            {
                throw new Exception("Fail request. catch:  " + ex.Message);
            }
        }

        /// <summary>
        /// Init replay the replay
        /// </summary>
        /// <param name="callback">Callback function</param>
        /// <param name="exchange">Name of exchange to replay</param>
        /// <param name="market">Name of market to replay</param>
        /// <param name="start_date">Date to start the replay</param>
        /// <param name="end_date">Date to end the replay</param>
        /// <param name="start">Set to true to start the replay directly</param>
        /// <returns></returns>
        public bool init_replay(CallBack callback, Exchange exchange, Market market, string start_date, string end_date, bool start = false)
        {
            // Init data
            this.callback = new CallBack(callback);
            this.replay_exchange = exchange;
            this.replay_market = market;
            this.replay_start_date = start_date;
            this.replay_end_date = end_date;

            if (start == true)
                start_replay();

            return true;
        }

        /// <summary>
        ///  Start the replay. Must be init before.
        /// </summary>
        public void start_replay()
        {
            if (thread_replay == null || thread_replay.IsAlive == false)
            {
                thread_replay = new Thread(work_replay);
                thread_shouldState = ShouldTread.Run;
                thread_replay.Start();
            }
            else
            {
                thread_shouldState = ShouldTread.Run;
            }
        }

        /// <summary>
        /// Stop or pause the replat
        /// </summary>
        /// <param name="definitely">definitely true means stop the replay.</param>
        public void stop_replay(bool definitely = true)
        {
            if (definitely == true)
                thread_shouldState = ShouldTread.End;
                        
            else
                thread_shouldState = ShouldTread.Wait; 
        }

        /// <summary>
        /// Get the full path of the cache directory
        /// </summary>
        /// <returns></returns>
        public string get_cache_directory()
        {
            return Path.GetFullPath("./cache-cl/");
        }
            
        // Work for the replay in new thread
        private void work_replay()
        {
            // List of date to replay
            List<string> dates_replayed = this.list_dates_replayed(this.replay_start_date, this.replay_end_date);

            // Download usefull files
            if (!this.download_files(this.replay_exchange, this.replay_market, this.replay_start_date, this.replay_end_date))
                return;

            // Init variables
            Dictionary<string, object> last_trade = new Dictionary<string, object>();

            // For each file, read
            foreach (string date in dates_replayed)
            {
                FileInfo file_trade = new FileInfo("./cache-cl/" + this.replay_exchange.exchange.ToLower() + "/" + this.replay_market.market.ToLower() + "/" + date + ".csv.gz");
                // Tests files
                if ((this.replay_type == null || this.replay_type == "trade") && file_trade.Exists == false)
                {
                    this.callback("init_replay - file trade " + file_trade.FullName + " doesn't exist.");
                    continue;
                }
                   
                // Read file
                List<Trade> trades = this.read_data(file_trade);

                // For each trade
                foreach (Trade trade in trades)
                {
                    // Callback event
                    this.callback(trade);

                    // Check if the user ask to stop the thread
                    while (thread_shouldState != ShouldTread.Run)
                    {
                        if (thread_shouldState == ShouldTread.End) // If end, quit function
                            return;
                        this.callback(null);
                        Thread.Sleep(100); // If wait, wait running
                    }
                }
                this.free_object(trades);
            }
            // End of replay
            return;
        }

        // Read file
        private List<Trade> read_data(FileInfo fileName)
        {
            // Init return var
            var trades = new List<Trade>();

            // Extract vsc from gzip
            string currentFileName = fileName.FullName;
            string tmp_output = "./cache-cl/tmp.csv";

            // Delete tmp_file
            if (System.IO.File.Exists(tmp_output))
                System.IO.File.Delete(tmp_output);

            byte[] dataBuffer = new byte[4096];
            using (System.IO.Stream fs = new FileStream(currentFileName, FileMode.Open, FileAccess.Read))
            {
                using (GZipInputStream gzipStream = new GZipInputStream(fs))
                {
                    using (FileStream fsOut = System.IO.File.Create(tmp_output))
                    {
                        StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
                    }
                }
            }

            // Configurer CSVHelper pour utiliser le séparateur de tabulation
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim
            };

            // Read CSV
            CsvReader csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(tmp_output)), config);

            // Read header
            csvReader.Read();
            csvReader.ReadHeader();
            while (csvReader.Read())
            {
                var tradeDictionary = new Dictionary<string, object>();

                foreach (var header in csvReader.HeaderRecord)
                {
                    tradeDictionary[header] = csvReader.GetField(header);
                }

                // Créer une instance de Trade et l'ajouter à la liste
                var trade = new Trade(tradeDictionary);
                trades.Add(trade);
            }
            csvReader.Dispose();
            return trades;
        }

            
        /// <summary>
        /// Download files for indaicated date
        /// </summary>
        /// <param name="exchange">Name of exchange to download</param>
        /// <param name="market">Name of market to download</param>
        /// <param name="start_date">Date of the first file to downlaod</param>
        /// <param name="end_date">Date of the last file to downlaod</param>
        /// <param name="type">Type of data ('trade' or 'orderbook'. Set to null for both)</param>
        /// <returns></returns>
        private bool download_files(Exchange exchange, Market market, string start_date, string end_date)
        {
            try
            {
                // Use API to get list of files
                List<File> list_files = this.get_files(exchange, market, start_date, end_date);
                FileInfo output_path = null;


                if (list_files.Count == 0)
                {
                    this.callback("No file with this parameters");
                    return false;
                }

                // For each file
                foreach (File file in list_files)
                {
                    // Output file
                    output_path = new FileInfo("./cache-cl/" + exchange.exchange.ToLower() + "/" + market.market.ToLower() + "/" + file.date + ".csv.gz");
                    // If exists, no need to download
                    if (output_path.Exists)
                        continue;
                    // File directory doesn't exist, create
                    if (!Directory.Exists(output_path.DirectoryName))
                        Directory.CreateDirectory(output_path.DirectoryName);

                    // Prepare request
                    RestRequest request = new RestRequest("data/file/" + exchange.exchange.ToLower() + "/" + market.market.ToLower() + "/" + file.date, Method.Get);
                    RestResponse response = client.Execute(request);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception("Status code: " + response.StatusCode);

                    // Save byte and close file
                    FileStream output = new FileStream(output_path.FullName, FileMode.Create);
                    output.Write(response.RawBytes);
                    output.Flush();
                    output.Close();

                    // Callback
                    this.callback("File download: " + exchange.exchange.ToLower() + " " + market.market.ToLower() + " " + file.date);
                }
                return true;
            }
            catch (Exception ex)
            {
                this.callback("download_files - error during files download: " + ex.Message);
                return false;
            }
        }

        // List of dates to raplay
        private List<string> list_dates_replayed(string start, string end)
        {
            DateTime start_date = DateTime.ParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime tmp_date = DateTime.ParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime end_date = DateTime.ParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            List<string> res = new List<string>();

            // Test dates
            if (start_date > end_date)
            {
                throw new Exception("start_date must be before end_date");
            }

            // Create list of dates
            while (tmp_date <= end_date)
            {
                res.Add(tmp_date.ToString("yyyy-MM-dd"));
                tmp_date = tmp_date.AddDays(1);
            }

            return res;
        }


        // Force to free memory for a list
        private void free_object(object obj)
        {
            int identificador = GC.GetGeneration(obj);
            obj = null;
            GC.Collect(identificador, GCCollectionMode.Forced);
        }
    }

}
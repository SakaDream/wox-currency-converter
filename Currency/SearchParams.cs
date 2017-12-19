using System;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using System.Net;

namespace Currency
{
    public class SearchParams
    {
        public decimal Amount { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }

        public SearchParams() { }
        public SearchParams(decimal amount, string fromCurrency)
        {
            Amount = amount;
            FromCurrency = fromCurrency;
            ToCurrency = GetCurrencyCode();
        }
        public SearchParams(decimal amount, string fromCurrency, string toCurrency)
        {
            Amount = amount;
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
        }
        public SearchParams(string fromCurrency, string toCurrency)
        {
            Amount = 1;
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
        }
        public SearchParams(string fromCurrency)
        {
            Amount = 1;
            FromCurrency = fromCurrency;
            ToCurrency = GetCurrencyCode();
        }

        private string GetCurrencyCode()
        {
            string countryCode;

            var url = $"http://ip-api.com/json";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            var respone = (HttpWebResponse)request.GetResponse();
            using (new StreamReader(respone.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                var responeString = new StreamReader(respone.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEnd();
                var json = JObject.Parse(responeString);
                countryCode = json["countryCode"].ToString();
            }
            try
            {
                RegionInfo ri = new RegionInfo(countryCode);
                return ri.ISOCurrencySymbol;
            }
            catch
            {
                return RegionInfo.CurrentRegion.ISOCurrencySymbol;
            }
        }
    }
}

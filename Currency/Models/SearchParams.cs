using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using System.Net;

namespace Currency.Models
{
    public class SearchParams
    {
        public decimal Amount { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }

        public SearchParams() { }
        public SearchParams(decimal Amount, string FromCurrency)
        {
            this.Amount = Amount;
            this.FromCurrency = FromCurrency;
            this.ToCurrency = GetCurrencyCode();
        }
        public SearchParams(decimal Amount, string FromCurrency, string ToCurrency)
        {
            this.Amount = Amount;
            this.FromCurrency = FromCurrency;
            this.ToCurrency = ToCurrency;
        }
        public SearchParams(string FromCurrency, string ToCurrency)
        {
            this.Amount = 1;
            this.FromCurrency = FromCurrency;
            this.ToCurrency = ToCurrency;
        }
        public SearchParams(string FromCurrency)
        {
            this.Amount = 1;
            this.FromCurrency = FromCurrency;
            this.ToCurrency = GetCurrencyCode();
        }

        private string GetCurrencyCode()
        {
            var CountryCode = "";

            var url = $"http://ip-api.com/json";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            var respone = (HttpWebResponse)request.GetResponse();
            using (new StreamReader(respone.GetResponseStream()))
            {
                var responeString = new StreamReader(respone.GetResponseStream()).ReadToEnd();
                var json = JObject.Parse(responeString);
                CountryCode = json["countryCode"].ToString();
            }
            try
            {
                RegionInfo ri = new RegionInfo(CountryCode);
                return ri.ISOCurrencySymbol;
            }
            catch
            {
                return RegionInfo.CurrentRegion.ISOCurrencySymbol;
            }
        }
    }
}

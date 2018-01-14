using System;
using System.Collections.Generic;
using System.Globalization;
using HtmlCrawler;
using Wox.Plugin;

namespace Currency
{
    class Utils
    {
        private Crawler _crawler;

        public Utils()
        {
            _crawler = new Crawler();
        }

        private bool CheckToCurrencyCode(string toCurrencyInput, string toCurrencyCrawled)
        {
            return toCurrencyInput.Equals(toCurrencyCrawled);
        }

        public decimal ParseAmount(string amount)
        {
            decimal result = 0;
            if (!Decimal.TryParse(amount, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                Decimal.TryParse(amount, NumberStyles.Any, new CultureInfo("en-US"), out result);
            }
            return result;
        }

        private decimal GetExchange(string fromCurrency, string toCurrency, decimal amount)
        {
            var url = $"http://www.xe.com/currencyconverter/convert/?Amount={amount}&From={fromCurrency}&To={toCurrency}";
            _crawler.UpdateDocument(url);
            var resultNode = _crawler.GetElementByClass("uccResultAmount");
            var toCurrencyCodeNode = _crawler.GetElementByClass("uccToCurrencyCode");
            return (CheckToCurrencyCode(toCurrency, toCurrencyCodeNode.InnerText))
                ? Decimal.Parse(resultNode.InnerText, new CultureInfo("en-US"))
                : -1;
        }

        public List<Result> GetResult(SearchParams sp)
        {
            var results = new List<Result>();

            var exchange = GetExchange(sp.FromCurrency, sp.ToCurrency, sp.Amount);

            if (exchange > 0)
            {
                results.Add(new Result
                {
                    Title = $"{sp.Amount:N} {sp.FromCurrency} = {exchange:N} {sp.ToCurrency}",
                    IcoPath = "Images/icon.png",
                    SubTitle = $"Source: http://www.xe.com/currencyconverter/"
                });
            }
            else
            {
                results.Add(new Result
                {
                    Title = $"Somethings went wrong...",
                    IcoPath = "Images/icon.png"
                });
            }
            //Debug: Add {APIDebug(sp.FromCurrency, sp.ToCurrency)} to Result.Title
            return results;
        }

        public List<Result> GetMessage(string message)
        {
            var results = new List<Result>();

            results.Add(new Result
            {
                Title = $"Error: {message}",
                IcoPath = "Images/icon.png"
            });

            return results;
        }

        public List<Result> GetMessage(string message, string desc)
        {
            var results = new List<Result>();

            results.Add(new Result
            {
                Title = $"Error: {message}",
                SubTitle = $"{desc}",
                IcoPath = "Images/icon.png"
            });

            return results;
        }
    }
    #region Debug
    //private string APIDebug(string fromCurrency, string toCurrency)
    //{
    //    string query = fromCurrency + "_" + toCurrency;
    //    var url = $"http://free.currencyconverterapi.com/api/v3/convert?q={query}&compact=ultra";
    //    var request = (HttpWebRequest)WebRequest.Create(url);
    //    request.Method = "GET";
    //    var respone = (HttpWebResponse)request.GetResponse();
    //    using (new StreamReader(respone.GetResponseStream()))
    //    {
    //        var responeString = new StreamReader(respone.GetResponseStream()).ReadToEnd();
    //        var json = JObject.Parse(responeString);
    //        return url + ", " + json[query];
    //    }
    //}

    //public List<Result> Debug(SearchParams sp)
    //{
    //    var results = new List<Result>();

    //    results.Add(new Result
    //    {
    //        Title = $"Debug: {sp.Amount}, {sp.FromCurrency}, {sp.ToCurrency}",
    //        IcoPath = "Images/icon.png"
    //    });

    //    return results;
    //}

    //public List<Result> Debug(string message)
    //{
    //    var results = new List<Result>();

    //    results.Add(new Result
    //    {
    //        Title = $"Debug: {message}",
    //        IcoPath = "Images/icon.png"
    //    });

    //    return results;
    //}

    //public List<Result> Debug(string message, string desc)
    //{
    //    var results = new List<Result>();

    //    results.Add(new Result
    //    {
    //        Title = $"Debug: {message}",
    //        SubTitle = $"{desc}",
    //        IcoPath = "Images/icon.png"
    //    });

    //    return results;
    //}

    //public List<Result> DebugJson(string url, string responeString)
    //{
    //    var results = new List<Result>();

    //    results.Add(new Result
    //    {
    //        Title = $"Debug: {url}, {responeString}",
    //        IcoPath = "Images/icon.png"
    //    });

    //    return results;
    //}
    #endregion
}

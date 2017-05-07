using Currency.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Wox.Plugin;

namespace Currency
{
    class CurrencyPlugin : IPlugin
    {
        private PluginInitContext _context;
        private readonly string currencyCheckPattern = @"cur\s([A-Za-z]{3})"; //cur usd
        private readonly string oneWaycheckPattern = @"^(\d+(\.\d{1,2})?)?\s([A-Za-z]{3})$"; //10 usd
        private readonly string inCheckPattern = @"(\d+(\.\d{1,2})?)?\s([A-Za-z]{3})\s([i][n])\s([A-Za-z]{3})"; // 10 usd in vnd
        private readonly string equalCheckPattern = @"(\d+(\.\d{1,2})?)?\s([A-Za-z]{3})\s([=])\s([A-Za-z]{3})"; // 10 usd = vnd

        public void Init(PluginInitContext context) { _context = context; }

        public List<Result> Query(Query query)
        {
            try
            {
                if(Regex.IsMatch(query.Search, currencyCheckPattern))
                {
                    if(query.RawQuery != null && query.RawQuery.Split(' ').Length == 2) // cur usd
                    {
                        SearchParams sp = new SearchParams(query.SecondSearch.ToUpper());
                        return GetResult(sp);
                    }
                    else
                    {
                        //return Debug("not currency way");
                        return new List<Result>();
                    }
                }
                else if(Regex.IsMatch(query.Search, oneWaycheckPattern))
                {
                    if (query.RawQuery != null && query.RawQuery.Split(' ').Length == 2) // 123 usd
                    {
                        SearchParams sp = new SearchParams(Convert.ToDecimal(query.FirstSearch), query.SecondSearch.ToUpper());
                        return GetResult(sp);
                    }
                    else
                    {
                        //return Debug("not one way");
                        return new List<Result>();
                    }
                }
                else if(Regex.IsMatch(query.Search, inCheckPattern))
                {
                    if (query.RawQuery != null && query.RawQuery.Split(' ').Length == 4) //123 usd in vnd
                    {
                        SearchParams sp = new SearchParams(
                            Convert.ToDecimal(query.FirstSearch), 
                            query.SecondSearch.ToUpper(), 
                            query.RawQuery.Split(' ')[3].ToUpper());
                        return GetResult(sp);
                    }
                    else
                    {
                        //return Debug("not in way");
                        return new List<Result>();
                    }
                }
                else if(Regex.IsMatch(query.Search, equalCheckPattern))
                {
                    if (query.RawQuery != null && query.RawQuery.Split(' ').Length == 4) //123 usd = vnd
                    {
                        SearchParams sp = new SearchParams(
                            Convert.ToDecimal(query.FirstSearch),
                            query.SecondSearch.ToUpper(),
                            query.RawQuery.Split(' ')[3].ToUpper());
                        return GetResult(sp);
                    }
                    else
                    {
                        //return Debug("not equal way");
                        return new List<Result>();
                    }
                }
                else
                {
                    //return Debug(query.RawQuery + ". Inside else");
                    return new List<Result>();
                }
            }
            catch(Exception e)
            {
                //return Debug(query.RawQuery + ". Inside catch: " + e.Message);
                return new List<Result>();
            }
        }

        private decimal GetRate(string fromCurrency, string toCurrency)
        {
            string query = fromCurrency + "_" + toCurrency;
            var url = $"http://free.currencyconverterapi.com/api/v3/convert?q={query}&compact=ultra";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            var respone = (HttpWebResponse)request.GetResponse();
            using (new StreamReader(respone.GetResponseStream()))
            {
                var responeString = new StreamReader(respone.GetResponseStream()).ReadToEnd();
                var json = JObject.Parse(responeString);
                return responeString.Equals("{ }") ? -1 : Decimal.Parse(json[query].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture);
            }
        }

        public List<Result> GetResult(SearchParams sp)
        {
            var results = new List<Result>();
            decimal rate = GetRate(sp.FromCurrency, sp.ToCurrency);
            if (rate != -1)
            {
                results.Add(new Result
                {
                    Title = $"{sp.Amount.ToString("N")} {sp.FromCurrency} = {(sp.Amount * rate).ToString("N")} {sp.ToCurrency}",
                    IcoPath = "Images/icon.png",
                    SubTitle = "Source: www.currencyconverterapi.com"
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
}

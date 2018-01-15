using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Wox.Plugin;

namespace Currency
{
    class CurrencyPlugin : IPlugin
    {
        private PluginInitContext _context;
        private Utils _utils;

        private readonly string currencyCheckPattern = @"cur\s([A-Za-z]{3})"; //cur usd
        private readonly string oneWaycheckPattern = @"^(\d+(\.\d{1,2})?)?\s([A-Za-z]{3})$"; //10 usd
        private readonly string inCheckPattern = @"(\d+(\.\d{1,2})?)?\s([A-Za-z]{3})\s([i][n])\s([A-Za-z]{3})"; // 10 usd in vnd
        private readonly string equalCheckPattern = @"(\d+(\.\d{1,2})?)?\s([A-Za-z]{3})\s([=])\s([A-Za-z]{3})"; // 10 usd = vnd

        public void Init(PluginInitContext context)
        {
            _context = context;
            _utils = new Utils();
        }

        public List<Result> Query(Query query)
        {
            try
            {
                if(Regex.IsMatch(query.Search, currencyCheckPattern))
                {
                    if(query.RawQuery != null && query.RawQuery.Split(' ').Length == 2) // cur usd
                    {
                        SearchParams sp = new SearchParams(query.SecondSearch.ToUpper());
                        return _utils.GetResult(sp);
                    }
                    else
                    {
                        return new List<Result>();
                    }
                }
                else if(Regex.IsMatch(query.Search, oneWaycheckPattern))
                {
                    if (query.RawQuery != null && query.RawQuery.Split(' ').Length == 2) // 123 usd
                    {
                        SearchParams sp = new SearchParams(_utils.ParseAmount(query.FirstSearch), query.SecondSearch.ToUpper());
                        return _utils.GetResult(sp);
                    }
                    else
                    {
                        return new List<Result>();
                    }
                }
                else if(Regex.IsMatch(query.Search, inCheckPattern))
                {
                    if (query.RawQuery != null && query.RawQuery.Split(' ').Length == 4) //123 usd in vnd
                    {
                        SearchParams sp = new SearchParams(
                            _utils.ParseAmount(query.FirstSearch), 
                            query.SecondSearch.ToUpper(), 
                            query.RawQuery.Split(' ')[3].ToUpper());
                        return _utils.GetResult(sp);
                    }
                    else
                    {
                        return new List<Result>();
                    }
                }
                else if(Regex.IsMatch(query.Search, equalCheckPattern))
                {
                    if (query.RawQuery != null && query.RawQuery.Split(' ').Length == 4) //123 usd = vnd
                    {
                        SearchParams sp = new SearchParams(
                            _utils.ParseAmount(query.FirstSearch),
                            query.SecondSearch.ToUpper(),
                            query.RawQuery.Split(' ')[3].ToUpper());
                        return _utils.GetResult(sp);
                    }
                    else
                    {
                        return new List<Result>();
                    }
                }
                else
                {
                    return new List<Result>();
                }
            }
            catch(Exception e)
            {
                return _utils.GetMessage("Something went wrong...", e.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.ToCurrency = RegionInfo.CurrentRegion.ISOCurrencySymbol;
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
            this.ToCurrency = RegionInfo.CurrentRegion.ISOCurrencySymbol;
        }
    }
}

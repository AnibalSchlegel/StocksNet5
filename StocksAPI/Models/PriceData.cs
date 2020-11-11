using StocksAPI.Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksAPI.Models
{
    public class PriceData
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal ClosingPrice { get; set; }
        public int Volume { get; set; }

        public int Symbol_ID { get; set; }

        [ForeignKey("Symbol_ID")]
        public virtual Symbol Symbol { get; set; }

        internal string ToCsvFormat(string symbol)
        {
            return $"{symbol}, {this.Date.ToShortDateString()}, {this.OpenPrice.TwoDecimalValues()}, {this.MinPrice.TwoDecimalValues()}, {this.MaxPrice.TwoDecimalValues()}, {this.ClosingPrice.TwoDecimalValues()}, {this.Volume}{Environment.NewLine}";
        }
    }
}
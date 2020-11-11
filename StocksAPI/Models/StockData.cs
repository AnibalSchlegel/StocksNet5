using System;

namespace StocksAPI.Models
{
    internal class StockData
    {
        public decimal ClosingPrice { get; set; }
        public DateTime Date { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal Volume { get; set; }
        public decimal ClosingPriceUSD { get; set; }
    }
}
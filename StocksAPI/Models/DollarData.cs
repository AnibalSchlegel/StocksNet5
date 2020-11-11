using System;

namespace StocksAPI.Models
{
    public class DollarData
    {
        public int ID { get; set; }
        public decimal Price { get; set; }
        public DateTime ExchangeDate { get; set; }
        public string DollarType { get; set; }
    }
}
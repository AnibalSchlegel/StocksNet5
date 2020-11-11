using System.Collections.Generic;

namespace StocksAPI.Models
{
    public class Symbol
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<PriceData> PriceSeries { get; set; }
    }
}
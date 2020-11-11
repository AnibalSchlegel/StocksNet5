namespace StocksNetCore3.Data
{
    public class Statistics
    {
        public string Symbol { get; set; }
        public decimal LastPrice { get; set; }
        public decimal LastPriceUsd { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AveragePriceUsd { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MaxPriceUsd { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MinPriceUsd { get; set; }
        public decimal AverageVolume { get; set; }
        public decimal MaxVolume { get; set; }
        public decimal LastVolume { get; set; }
        public decimal VsMaxPrice { get; set; }
        public decimal VsMaxPriceUsd { get; set; }
        public decimal VsAvgPrice { get; set; }
        public decimal VsAvgPriceUsd { get; set; }
        public decimal VsMinPriceUsd { get; set; }
    }
}
using StocksAPI.Extensions;
using System;

namespace StocksAPI.Models
{
    public class Statistics : IComparable<Statistics>
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
        public decimal MinVolume { get; set; }
        public decimal LastVolume { get; set; }

        public decimal VsMaxPrice {
            get
            {
                if (this.MaxPrice == this.LastPrice)
                    return new decimal(0).ZeroDecimalValues();
                else
                {
                    var exp = ((this.LastPrice / this.MaxPrice) - 1) * 100;
                    return exp.ZeroDecimalValues();
                }
            }
        }
        public decimal VsMaxPriceUsd
        {
            get
            {
                if (this.MaxPriceUsd == this.LastPriceUsd)
                    return new decimal(0);
                else
                {
                    var exp = ((this.LastPriceUsd / this.MaxPriceUsd) - 1) * 100;
                    return exp.ZeroDecimalValues();
                }
            }
        }

        public decimal VsAvgPrice
        {
            get
            {
                if (this.AveragePrice == this.LastPrice)
                    return new decimal(0);
                else if(this.AveragePrice > this.LastPrice)
                {
                    var exp = ((this.LastPrice / this.AveragePrice) - 1) * 100;
                    return exp.ZeroDecimalValues();
                }
                else
                {
                    var exp = (this.LastPrice / this.AveragePrice) * 100;
                    return exp.ZeroDecimalValues();
                }
            }
        }
        public decimal VsAvgPriceUsd
        {
            get
            {
                if (this.AveragePriceUsd == this.LastPriceUsd)
                    return new decimal(0).ZeroDecimalValues();
                else if (this.AveragePriceUsd > this.LastPriceUsd)
                {
                    var exp = ((this.LastPriceUsd / this.AveragePriceUsd) - 1) * 100;
                    return exp.ZeroDecimalValues();
                }
                else
                {
                    var exp = (this.LastPriceUsd / this.AveragePriceUsd) * 100; ;
                    return exp.ZeroDecimalValues();
                }
            }
        }

        public decimal VsMinPriceUsd => this.LastPriceUsd - this.MinPriceUsd;

        public int CompareTo(Statistics other) => VsMinPriceUsd.CompareTo(other.VsMinPriceUsd);
    }
}
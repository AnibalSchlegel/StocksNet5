using StocksAPI.Extensions;
using System;

namespace StocksAPI.Models
{
    public class Statistics : IComparable<Statistics>
    {
        public Statistics(decimal averagePrice, decimal averagePriceUsd, decimal averageVolume, decimal lastPrice, decimal lastPriceUsd, decimal lastVolume, decimal maxPrice,
            decimal maxPriceUsd, decimal maxVolume, decimal minPrice, decimal minPriceUsd)
        {
            AveragePrice = averagePrice;
            AveragePriceUsd = averagePriceUsd;
            AverageVolume = averageVolume;
            LastPrice = lastPrice;
            LastPriceUsd = lastPriceUsd;
            LastVolume = lastVolume;
            MaxPrice = maxPrice;
            MaxPriceUsd = maxPriceUsd;
            MaxVolume = maxVolume;
            MinPrice = minPrice;
            MinPriceUsd = minPriceUsd;
        }

        //public int LongestDownTrend { get { return this. } }
        //public int LongestUpTrend { get { return this. } }
        public string Symbol { get; set; }
        public decimal LastPrice { get; }
        public decimal LastPriceUsd { get; }
        public decimal AveragePrice { get; }
        public decimal AveragePriceUsd { get; }
        public decimal MaxPrice { get; }
        public decimal MaxPriceUsd { get; }
        public decimal MinPrice { get; }
        public decimal MinPriceUsd { get; }
        public decimal AverageVolume { get; }
        public decimal MaxVolume { get; }
        public decimal LastVolume { get; }

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
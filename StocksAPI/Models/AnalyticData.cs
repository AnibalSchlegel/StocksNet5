using System;
using System.Collections.Generic;

namespace StocksAPI.Models
{
    public class AnalyticData
    {
        public string Symbol { get; set; }
        public int RoundsCount { get; set; }
        public int UpsideRoundsCount { get; set; }
        public int DownsideRoundsCount { get; set; }
        public int AvgUpsideStreakCount { get; set; }
        public int AvgDownsideStreakCount { get; set; }
        public int MaxUpsideStreakCount { get; set; }
        public int MaxDownsideStreakCount { get; set; }
        public long MaxVolume { get; set; }
        public long AvgVolume { get; set; }
        public long MinVolume { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal MinPrice { get; set; }
        public DateTime MaxPriceDate { get; set; }
        public DateTime MinPriceDate { get; set; }
        public DateTime MaxVolumeDate { get; set; }
        public DateTime MinVolumeDate { get; set; }
        public List<string> Sequence { get; set; }
    }
}

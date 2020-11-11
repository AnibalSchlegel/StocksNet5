using StocksAPI.Extensions;
using StocksAPI.Models;
using System;
using System.Collections.Generic;

namespace StocksAPI.Calculations
{
    public class AnalyticProcessor
    {
        string Symbol { get; set; }
        List<PriceData> PriceSeries { get; set; }

        public AnalyticProcessor(string symbol, List<PriceData> priceData)
        {
            Symbol = symbol;
            PriceSeries = priceData;
        }

        public AnalyticData ProcessData()
        {
            AnalyticData data = new AnalyticData();
            Streak currentStreak = Streak.Up;
            Streak previousRound = Streak.NoData;
            int currentStreakCount = 0;
            decimal priceAccumulator = 0;
            long volumeAccumulator = 0;
            Dictionary<int, int> sequenceDist = new Dictionary<int, int>();

            data.Symbol = Symbol;
            data.RoundsCount = this.PriceSeries.Count;
            InitAnalytics(data);

            foreach (PriceData round in PriceSeries)
            {
                priceAccumulator += RoundPriceAvg(round);
                volumeAccumulator += round.Volume;

                MinsMaxs(data, round);

                StreakCalculator(data, ref currentStreak, ref previousRound, ref currentStreakCount, round, sequenceDist);

            }

            CalculateAverages(sequenceDist, data, volumeAccumulator, priceAccumulator, data.RoundsCount);

            return data;
        }

        private void CalculateAverages(Dictionary<int, int> sequenceDist, AnalyticData data, long volumeAccumulator, decimal priceAccumulator, int roundCount)
        {
            data.AvgVolume = volumeAccumulator / roundCount;
            data.AvgPrice = (priceAccumulator / roundCount).TwoDecimalValues();

            //int upCount = 0;
            //int upSum = 0;
            //int downCount = 0;
            //int downSum = 0;

            //foreach (int streak in sequenceDist.Keys)
            //{
            //    if(streak < 0)
            //    {
            //        downCount++;
            //        downSum += sequenceDist[streak];
            //    }
            //}
        }

        private void StreakCalculator(AnalyticData data, ref Streak currentStreak, ref Streak previousRound, ref int currentStreakCount, PriceData round, Dictionary<int,int> sequenceDist)
        {
            if (round.ClosingPrice > round.OpenPrice)
            {
                data.UpsideRoundsCount++;

                if (currentStreak == Streak.Down && previousRound == Streak.Up)
                {
                    data.Sequence.Add($"{currentStreakCount}U");
                    UpdateSequenceDictionary(sequenceDist, -currentStreakCount);
                    if (currentStreakCount > data.MaxDownsideStreakCount)
                        data.MaxDownsideStreakCount = currentStreakCount;
                    currentStreakCount = 2;
                    currentStreak = Streak.Up;
                }
                else
                {
                    currentStreakCount++;
                }

                previousRound = Streak.Up;
            }
            else if (round.ClosingPrice <= round.OpenPrice)
            {
                data.DownsideRoundsCount++;

                if (currentStreak == Streak.Up && previousRound == Streak.Down)
                {
                    data.Sequence.Add($"{currentStreakCount}D");
                    UpdateSequenceDictionary(sequenceDist, currentStreakCount);
                    if (currentStreakCount > data.MaxUpsideStreakCount)
                        data.MaxUpsideStreakCount = currentStreakCount;
                    currentStreakCount = 2;
                    currentStreak = Streak.Down;
                }
                else
                {
                    currentStreakCount++;
                }

                previousRound = Streak.Down;
            }
        }

        private void InitAnalytics(AnalyticData data)
        {
            data.AvgDownsideStreakCount = 0;
            data.AvgUpsideStreakCount = 0;
            data.DownsideRoundsCount = 0;
            data.UpsideRoundsCount = 0;
            data.MaxDownsideStreakCount = 0;
            data.MaxUpsideStreakCount = 0;
            data.MaxPrice = 0;
            data.MaxVolume = 0;
            data.MinPrice = decimal.MaxValue;
            data.MinVolume = long.MaxValue;
            data.Sequence = new List<string>();
        }

        private void UpdateSequenceDictionary(Dictionary<int, int> sequenceDict, int currentStreakCount)
        {
            if (!sequenceDict.ContainsKey(currentStreakCount))
                sequenceDict.Add(currentStreakCount, 0);

            sequenceDict[currentStreakCount]++;
        }

        private void MinsMaxs(AnalyticData data, PriceData round)
        {
            if (round.MaxPrice >= data.MaxPrice)
            {
                data.MaxPrice = round.MaxPrice;
                data.MaxPriceDate = round.Date;
            }
            if (round.MinPrice <= data.MinPrice)
            {
                data.MinPrice = round.MinPrice;
                data.MinPriceDate = round.Date;
            }
            if (round.MinPrice <= data.MinPrice)
            {
                data.MinPrice = round.MinPrice;
                data.MinPriceDate = round.Date;
            }
            if (round.Volume >= data.MaxVolume)
            {
                data.MaxVolume = round.Volume;
                data.MaxVolumeDate = round.Date;
            }
            if (round.Volume <= data.MinVolume)
            {
                data.MinVolume = round.Volume;
                data.MinVolumeDate = round.Date;
            }
        }

        private decimal RoundPriceAvg(PriceData data) => (data.ClosingPrice + data.MaxPrice + data.MinPrice + data.OpenPrice) / 4;

        private enum Streak
        {
            Up,
            Down,
            NoData
        }
    }
}

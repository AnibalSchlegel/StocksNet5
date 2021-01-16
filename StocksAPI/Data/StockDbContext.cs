using Microsoft.EntityFrameworkCore;
using StocksAPI.Calculations;
using StocksAPI.Extensions;
using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksAPI.Data
{
    public sealed class StockDbContext: DbContext
    {
        public StockDbContext(DbContextOptions options): base(options)
            { }

        public DbSet<Symbol> Symbol { get; set; }
        public DbSet<PriceData> PriceData { get; set; }
        public DbSet<DollarData> DollarData { get; set; }
        public DbSet<Status> Status { get; set; }

        #region Queries

        public async Task<AnalyticData> GetAnalytics(string symbol, string currency = "PESOS")
        {
            var priceSeries = new List<PriceData>();

            var symbolId = this.Symbol.Where(x => x.Name == symbol).FirstOrDefault().ID;

            if (currency.Equals("PESOS"))
            {
                priceSeries = await PriceData.Where(x => x.Symbol_ID == symbolId).OrderBy(x => x.Date).ToListAsync();
            }
            else
            {
                priceSeries = await this.PriceData.Where(ps => ps.Symbol.ID == symbolId)
                    .Join(this.DollarData.Where(ds => ds.DollarType == currency), ps => ps.Date, ds => ds.ExchangeDate,
                (ps, ds) => new PriceData
                {
                   ClosingPrice = (ps.ClosingPrice / ds.Price),
                   Date = ps.Date,
                   MaxPrice = (ps.MaxPrice / ds.Price),
                   MinPrice = (ps.MinPrice / ds.Price),
                   OpenPrice = (ps.OpenPrice / ds.Price),
                   Volume = ps.Volume
                }).OrderBy(x=>x.Date).ToListAsync();
            }

            return new AnalyticProcessor(symbol, priceSeries).ProcessData();
        }

        public async Task<DateTime> GetLastUpdateDate()
        {
            var status = await this.Status.FirstOrDefaultAsync();

            if (status != null)
            {
                return status.LastUpdate;
            }

            return default;
        }

        public async Task<DateTime> GetLastKnownDateForDollarData()
        {
            var date = await this.DollarData.OrderByDescending(z => z.ExchangeDate).Select(y => y.ExchangeDate).FirstOrDefaultAsync();

            if (date == default)
            {
                date = new DateTime(2005, 01, 01);
            }
            return date;
        }

        public async Task<DateTime?> GetLastKnownDateForSymbol(int id)
        {
            return await 
                this.PriceData
                .Where(x => x.Symbol.ID == id)
                .OrderByDescending(z => z.Date)
                .Select(y => y.Date)
                .FirstOrDefaultAsync();
        }

        public async void UpdateStatus()
        {
            if (this.Status.Count() == 1)
            {
                var st = this.Status.First();
                st.LastUpdate = DateTime.Now;
                this.Status.Add(st);
                this.Entry(st).State = EntityState.Modified;
            }
            else
            {
                await this.Status.AddAsync(new Status { LastUpdate = DateTime.Now });
            }
        }

        public async Task<byte[]> GetPriceDataAsCsv(string symbol = "", string currency = "CCL")
        {
            var priceSeries = new List<PriceData>();

            var symbolId = symbol != null ? this.Symbol.Where(x => x.Name == symbol).FirstOrDefaultAsync().Result.ID : -1;

            if (currency.Equals("PESOS"))
            {
                priceSeries = symbolId > 0 
                    ? await PriceData.Where(x => x.Symbol_ID == symbolId).OrderBy(x => x.Date).ToListAsync() 
                    : await PriceData.OrderBy(x => x.Date).ToListAsync();
            }
            else
            {
                var query = symbolId > 0 ? this.PriceData.Where(ps => ps.Symbol.ID == symbolId) : this.PriceData;

                priceSeries = await query.Join(this.DollarData.Where(ds => ds.DollarType == currency), ps => ps.Date, ds => ds.ExchangeDate,
               (ps, ds) => new PriceData
               {
                   ClosingPrice = (ps.ClosingPrice / ds.Price),
                   Date = ps.Date,
                   MaxPrice = (ps.MaxPrice / ds.Price),
                   MinPrice = (ps.MinPrice / ds.Price),
                   OpenPrice = (ps.OpenPrice / ds.Price),
                   Volume = ps.Volume
               }).OrderBy(x => x.Date).ToListAsync();
            }

            return priceSeries.SelectMany(s => Encoding.UTF8.GetBytes(s.ToCsvFormat(symbol))).ToArray();
        }

        public Statistics GetStatistics(int symbolId, DateTime dateFrom = default, DateTime? dateTo = null, string dollarType = "CCL")
        {
            if (!dateTo.HasValue)
                dateTo = DateTime.Now;

            var data = this.PriceData.Where(ps => ps.Symbol.ID == symbolId && ps.Date >= dateFrom && ps.Date <= dateTo)
                .Join(this.DollarData.Where(ds => ds.DollarType == dollarType), ps => ps.Date, ds => ds.ExchangeDate,
                (ps, ds) => new StockData
                {
                    ClosingPrice = ps.ClosingPrice,
                    Date = ps.Date,
                    MaxPrice = ps.MaxPrice,
                    MinPrice = ps.MinPrice,
                    OpenPrice = ps.OpenPrice,
                    Volume = ps.Volume,
                    ClosingPriceUSD = (ps.ClosingPrice / ds.Price)
                });

            var stats = new Statistics {
                AveragePrice = data.Average(a => a.ClosingPrice).TwoDecimalValues(),                          
                AveragePriceUsd = data.Average(au => au.ClosingPriceUSD).TwoDecimalValues(),                  
                AverageVolume = data.Average(v => v.Volume).ZeroDecimalValues(),                              
                LastPrice = data.OrderByDescending(d => d.Date).ToList().First().ClosingPrice.TwoDecimalValues(),      
                LastPriceUsd = data.OrderByDescending(d => d.Date).ToList().First().ClosingPriceUSD.TwoDecimalValues(),
                LastVolume = data.OrderByDescending(d => d.Date).ToList().First().Volume.ZeroDecimalValues(), 
                MaxPrice = data.Max(m => m.ClosingPrice).TwoDecimalValues(),      
                MaxPriceUsd = data.Max(m => m.ClosingPriceUSD).TwoDecimalValues(),
                MaxVolume = data.Max(m => m.Volume).ZeroDecimalValues(),          
                MinPrice = data.Min(m => m.ClosingPrice).TwoDecimalValues(),      
                MinPriceUsd = data.Min(m => m.ClosingPriceUSD).TwoDecimalValues(),
                MinVolume = data.Min(m => m.Volume).ZeroDecimalValues()
            };

            var temp = data.Where(d => d.Date >= DateTime.Today.AddDays(-20)).ToList();

            return stats;
        }

        public async Task<List<Statistics>> GetAllStatistics(DateTime dateFrom = default, DateTime? dateTo = null, string dollarType = "CCL")
        {
            if (!dateTo.HasValue)
                dateTo = DateTime.Now;

            List<Statistics> list = new List<Statistics>();

            await this.Symbol.ForEachAsync(symbol =>
            {
                var stat = GetStatistics(symbol.ID, dateFrom, dateTo, dollarType);
                stat.Symbol = symbol.Name;
                list.Add(stat);
            });
            list.Sort();
            return list;
        }

        #endregion
    }
}

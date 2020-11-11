﻿using Microsoft.EntityFrameworkCore;
using StocksAPI.Calculations;
using StocksAPI.Extensions;
using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksAPI.Data
{
    public class StockDbContext: DbContext
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
                priceSeries = PriceData.Where(x => x.Symbol_ID == symbolId).OrderBy(x => x.Date).ToList();
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

        public DateTime GetLastKnownDateForDollarData()
        {
            var date = this.DollarData.OrderByDescending(z => z.ExchangeDate).Select(y => y.ExchangeDate).FirstOrDefault();

            if (date == default)
            {
                date = new DateTime(2005, 01, 01);
            }
            return date;
        }

        public DateTime? GetLastKnownDateForSymbol(int id)
        {
            return this.PriceData.Where(x => x.Symbol.ID == id).OrderByDescending(z => z.Date).Select(y => y.Date).FirstOrDefault();
        }

        public void UpdateStatus()
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
                this.Status.Add(new Status { LastUpdate = DateTime.Now });
            }
        }

        public async Task<byte[]> GetPriceDataAsCsv(string symbol = "", string currency = "CCL")
        {
            var priceSeries = new List<PriceData>();

            var symbolId = symbol != null ? this.Symbol.Where(x => x.Name == symbol).FirstOrDefault().ID : -1;

            if (currency.Equals("PESOS"))
            {
                priceSeries = symbolId > 0 ? await PriceData.Where(x => x.Symbol_ID == symbolId).OrderBy(x => x.Date).ToListAsync() : await PriceData.OrderBy(x => x.Date).ToListAsync();
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

            var stats = new Statistics(
                data.Average(a => a.ClosingPrice).TwoDecimalValues(),
                data.Average(au => au.ClosingPriceUSD).TwoDecimalValues(),
                data.Average(v => v.Volume).ZeroDecimalValues(),
                data.OrderByDescending(d => d.Date).First().ClosingPrice.TwoDecimalValues(),
                data.OrderByDescending(d => d.Date).First().ClosingPriceUSD.TwoDecimalValues(),
                data.OrderByDescending(d => d.Date).First().Volume.ZeroDecimalValues(),
                data.Max(m => m.ClosingPrice).TwoDecimalValues(),
                data.Max(m => m.ClosingPriceUSD).TwoDecimalValues(),
                data.Max(m => m.Volume).ZeroDecimalValues(),
                data.Min(m => m.ClosingPrice).TwoDecimalValues(),
                data.Min(m => m.ClosingPriceUSD).TwoDecimalValues()
            );

            return stats;
        }

        public async Task<List<Statistics>> GetAllStatistics(DateTime dateFrom = default, DateTime? dateTo = null, string dollarType = "CCL")
        {
            if (!dateTo.HasValue)
                dateTo = DateTime.Now;

            List<Statistics> list = new List<Statistics>();

            await foreach (Symbol s in this.Symbol.OrderBy(s => s.Name).AsAsyncEnumerable())
            {
                var stat = GetStatistics(s.ID, dateFrom, dateTo, dollarType);
                stat.Symbol = s.Name;
                list.Add(stat);
            }
            list.Sort();
            return list;
        }

        #endregion
    }
}
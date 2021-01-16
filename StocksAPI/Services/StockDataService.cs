using Microsoft.EntityFrameworkCore;
using StocksAPI.Data;
using StocksAPI.ExternalDataProviders;
using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksAPI.Services
{
    public class StockDataService : IStockDataService
    {
        readonly IDbContextFactory<StockDbContext> contextFactory;
        readonly IStockExternalDataProvider stockDataProvider;
        readonly IDollarDataProvider dollarDataProvider;

        public StockDataService(IDbContextFactory<StockDbContext> contextFactory, IStockExternalDataProvider stockDataProvider, IDollarDataProvider dollarDataProvider)
        {
            this.contextFactory = contextFactory;
            this.dollarDataProvider = dollarDataProvider;
            this.stockDataProvider = stockDataProvider;
        }

        public async Task<byte[]> GetSymbolDataForCsv(string symbol, string currency)
        {
            using var context = contextFactory.CreateDbContext();
            return await context.GetPriceDataAsCsv(symbol, currency);
        }

        public async Task<List<Symbol>> GetAllSymbols()
        {
            using var context = contextFactory.CreateDbContext();
            return await context.Set<Symbol>().OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<AnalyticData> GetAnalytics(string symbol, string currency)
        {
            using var context = contextFactory.CreateDbContext();
            return await context.GetAnalytics(symbol, currency);
        }

        public async Task<DateTime> GetLastUpdateDate()
        {
            using var context = contextFactory.CreateDbContext();
            return await context.GetLastUpdateDate();
        }

        public async Task<List<Statistics>> GetAllStatistics(DateTime dateFrom, DateTime dateTo, string dollarType = "CCL")
        {
            using var context = contextFactory.CreateDbContext();
            return await context.GetAllStatistics(dateFrom, dateTo, dollarType);
        }

        public async Task<bool> Update()
        {
            DateTime date;
            List<Symbol> symbols = new List<Symbol>();

            try
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    date = await context.GetLastKnownDateForDollarData();
                    symbols = context.Symbol.ToList();

                    symbols.ForEach(symbol =>
                    {
                        DateTime? lastDate = context.GetLastKnownDateForSymbol(symbol.ID).Result;
                        context.PriceData.AddRange(stockDataProvider.DownloadPriceSeries(symbol, lastDate).Result);
                        context.UpdateStatus();
                        context.SaveChanges();

                    });
                    context.DollarData.AddRange(dollarDataProvider.DownloadDollarData(date).Result);
                    context.UpdateStatus();
                    context.SaveChanges();
                }

                return true;
            }
            catch(Exception ex)
            {
                var message = ex.Message;
                return false;
            }
        }
    }
}

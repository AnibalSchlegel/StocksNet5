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
        readonly StockDbContext context;
        readonly IStockExternalDataProvider stockDataProvider;
        readonly IDollarDataProvider dollarDataProvider;

        public StockDataService(StockDbContext context, IStockExternalDataProvider stockDataProvider, IDollarDataProvider dollarDataProvider)
        {
            this.context = context;
            this.dollarDataProvider = dollarDataProvider;
            this.stockDataProvider = stockDataProvider;
        }

        public async Task<byte[]> GetSymbolDataForCsv(string symbol, string currency)
        {
            return await context.GetPriceDataAsCsv(symbol, currency);
        }

        public async Task<List<Symbol>> GetAllSymbols()
        {
            return await context.Set<Symbol>().OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<AnalyticData> GetAnalytics(string symbol, string currency)
        {
            return await context.GetAnalytics(symbol, currency);
        }

        public async Task<DateTime> GetLastUpdateDate()
        {
            return await context.GetLastUpdateDate();
        }

        public async Task<List<Statistics>> GetAllStatistics(DateTime dateFrom, DateTime dateTo, string dollarType = "CCL")
        {
            return await context.GetAllStatistics(dateFrom, dateTo, dollarType);
        }

        public async Task<bool> Update()
        {
            try
            {
                var date = await context.GetLastKnownDateForDollarData();

                foreach (DollarData dd in await dollarDataProvider.DownloadDollarData(date))
                {
                    this.context.DollarData.Add(dd);
                }
                foreach (Symbol s in context.Symbol)
                {
                    DateTime? lastDate = await context.GetLastKnownDateForSymbol(s.ID);
                    this.context.PriceData.AddRange(await stockDataProvider.DownloadPriceSeries(s, lastDate));
                }

                context.UpdateStatus();
                await context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

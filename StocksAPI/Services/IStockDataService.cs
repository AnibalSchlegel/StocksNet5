using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StocksAPI.Services
{
    public interface IStockDataService
    {
        Task<bool> Update();

        Task<List<Symbol>> GetAllSymbols();

        Task<DateTime> GetLastUpdateDate();

        Task<AnalyticData> GetAnalytics(string symbol, string currency);

        Task<byte[]> GetSymbolDataForCsv(string symbol, string currency);

        Task<List<Statistics>> GetAllStatistics(DateTime dateFrom, DateTime dateTo, string dollarType = "CCL");
    }
}

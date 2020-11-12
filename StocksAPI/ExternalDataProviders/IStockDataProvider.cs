using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksAPI.ExternalDataProviders
{
    public interface IStockDataProvider
    {
        Task<List<PriceData>> DownloadPriceSeries(Symbol symbol, DateTime? lastKnownDate);
    }
}

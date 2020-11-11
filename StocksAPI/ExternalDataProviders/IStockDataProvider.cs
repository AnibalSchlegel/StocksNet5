using StocksAPI.Models;
using System;
using System.Collections.Generic;

namespace StocksAPI.ExternalDataProviders
{
    public interface IStockDataProvider
    {
        List<PriceData> DownloadPriceSeries(Symbol symbol, DateTime? lastKnownDate);
    }
}

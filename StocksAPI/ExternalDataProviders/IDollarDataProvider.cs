using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksAPI.ExternalDataProviders
{
    public interface IDollarDataProvider
    {
        Task<List<DollarData>> DownloadDollarData(DateTime lastKnownPrice);
    }
}
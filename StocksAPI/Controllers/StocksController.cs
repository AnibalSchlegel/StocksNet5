using Microsoft.AspNetCore.Mvc;
using StocksAPI.Data;
using StocksAPI.ExternalDataProviders;
using StocksAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StocksAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StocksController : ControllerBase
    {
        readonly StockDbContext context;
        readonly IStockDataProvider stockDataProvider;
        readonly IDollarDataProvider dollarDataProvider;

        public StocksController(StockDbContext context, IStockDataProvider stockDataProvider, IDollarDataProvider dollarDataProvider)
        {
            this.context = context;
            this.dollarDataProvider = dollarDataProvider;
            this.stockDataProvider = stockDataProvider;
        }

        [HttpGet("Update")]
        public async Task<IActionResult> Update()
        {
            try
            {
                var status = await context.GetLastUpdateDate();

                if (status >= DateTime.Today)
                    return Ok("System already up-to-date.");

                var date = context.GetLastKnownDateForDollarData();

                foreach (DollarData dd in await dollarDataProvider.DownloadDollarData(date))
                {
                    this.context.DollarData.Add(dd);
                }
                foreach (Symbol s in context.Symbol)
                {
                    DateTime? lastDate = context.GetLastKnownDateForSymbol(s.ID);
                    this.context.PriceData.AddRange(stockDataProvider.DownloadPriceSeries(s, lastDate));
                }

                context.UpdateStatus();
                await context.SaveChangesAsync();

                return Ok("System updated successfully.");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("Symbols")]
        public IActionResult GetSymbols()
        {
            try
            {
                return Ok(context.Set<Symbol>().OrderBy(x=>x.Name).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Analytics/{symbol}/{currency}")]
        public async Task<IActionResult> GetAnalytics(string symbol, string currency)
        {
            try
            {
                if (string.IsNullOrEmpty(symbol))
                    return BadRequest("Invalid parameter: symbol.");
                if (string.IsNullOrEmpty(currency) || (currency != "CCL" && currency != "OFICIAL"))
                    currency = "PESOS";
                return Ok(await context.GetAnalytics(symbol, currency));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetCsv/{symbol}/{currency}")]
        public async Task<IActionResult> GetCsv(string symbol = "", string currency = "CCL")
        {
            try
            {
                if (string.IsNullOrEmpty(symbol))
                    symbol = "ALL";
                if (string.IsNullOrEmpty(currency) || (currency != "CCL" && currency != "OFICIAL"))
                    currency = "PESOS";
                var data = await context.GetPriceDataAsCsv(symbol, currency);
                var result = new FileContentResult(data, "application/octet-stream")
                {
                    FileDownloadName = $"{symbol}_{currency}.csv"
                };
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("LastUpdate")]
        public async Task<IActionResult> GetLastUpdateDate()
        {
            try
            {
                return Ok(await context.GetLastUpdateDate());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetStats/{dollarType}")]
        public async Task<IActionResult> GetStats(string dollarType)
        {
            try
            {
                if(!string.IsNullOrEmpty(dollarType))
                    return Ok(await context.GetAllStatistics(default,DateTime.Today,dollarType));
                else
                    return Ok(await context.GetAllStatistics(default, DateTime.Today));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

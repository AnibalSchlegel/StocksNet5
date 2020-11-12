using Microsoft.AspNetCore.Mvc;
using StocksAPI.Models;
using StocksAPI.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StocksAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StocksController : ControllerBase
    {
        readonly IStockDataService stockDataService;

        public StocksController(IStockDataService stockDataService)
        {
            this.stockDataService = stockDataService;
        }

        [HttpGet("Update")]
        public async Task<IActionResult> Update()
        {
            try
            {
                var lastUpdateDate = await stockDataService.GetLastUpdateDate();

                if (lastUpdateDate >= DateTime.Today)
                    return Ok("System already up-to-date.");

                bool updateResult = await stockDataService.Update();

                if (updateResult)
                    return Ok("System updated successfully.");
                else
                    return Conflict("An error has ocurred while updating the stock data.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Symbols")]
        public async Task<IActionResult> GetSymbols()
        {
            try
            {
                return Ok(await stockDataService.GetAllSymbols());
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
               
                return Ok(await stockDataService.GetAnalytics(symbol, currency));
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

                var data = await stockDataService.GetSymbolDataForCsv(symbol, currency);
                
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
                return Ok(await stockDataService.GetLastUpdateDate());
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
                    return Ok(await stockDataService.GetAllStatistics(default, DateTime.Today, dollarType));
                else
                    return Ok(await stockDataService.GetAllStatistics(default, DateTime.Today));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

using StocksAPI.Extensions;
using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace StocksAPI.ExternalDataProviders
{
    public class RavaDataProvider : IStockDataProvider
    {
        const string BASE_URL = "http://ravaonline.com/empresas/precioshistoricos.php?e={0}&csv=1";

        public async Task<List<PriceData>> DownloadPriceSeries(Symbol symbol, DateTime? lastKnownDate)
        {
            if (lastKnownDate.HasValue && lastKnownDate == DateTime.Today)
                return new List<PriceData>();

            WebRequest request = WebRequest.Create(string.Format(BASE_URL, symbol.Name));
            WebResponse response = await request.GetResponseAsync();

            var streamReader = new StreamReader(response.GetResponseStream());
            
            if (streamReader == null)
                return new List<PriceData>();

            return ParseResponseStream(streamReader, lastKnownDate, symbol);
        }

        private List<PriceData> ParseResponseStream(StreamReader streamReader, DateTime? lastKnownDate, Symbol symbol)
        {
            List<PriceData> parsedData = new List<PriceData>();

            while (!streamReader.EndOfStream)
            {
                var newTicketData = ParseSingleLine(streamReader.ReadLine(), lastKnownDate, symbol);

                if (newTicketData != null)
                    parsedData.Add(newTicketData);
            }

            return parsedData;
        }

        private PriceData ParseSingleLine(string line, DateTime? lastKnownDate, Symbol symbol)
        {
            try
            {
                string[] data = line.Split(',');

                PriceData day = new PriceData();
                day.Date = DateTime.Parse(data[0].Replace("\"", ""));

                if (lastKnownDate.HasValue && lastKnownDate.Value >= day.Date) return null;

                day.OpenPrice = ParseSegment(data[1]);
                day.MaxPrice = ParseSegment(data[2]);
                day.MinPrice = ParseSegment(data[3]);
                day.ClosingPrice = ParseSegment(data[4]);
                day.Volume = int.Parse(data[5].Replace("\"", ""));
                day.Symbol = symbol;

                if (day.OpenPrice == 0 || day.ClosingPrice == 0)
                    return null;

                return day;
            }
            catch
            {
                return null;
            }
        }

        private decimal ParseSegment(string segment)
        {
            return Convert.ToDecimal(segment.Replace("\"", "").Replace(".", ","), CultureInfo.GetCultureInfo("es")).TwoDecimalValues();
        }
    }
}
using StocksAPI.Extensions;
using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;

namespace StocksAPI.ExternalDataProviders
{
    public class RavaDataProvider : IStockDataProvider
    {
        const string BASE_URL = "http://ravaonline.com/empresas/precioshistoricos.php?e={0}&csv=1";

        public List<PriceData> DownloadPriceSeries(Symbol symbol, DateTime? lastKnownDate)
        {
            if (lastKnownDate.HasValue && lastKnownDate == DateTime.Today)
                return new List<PriceData>();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(BASE_URL, symbol.Name));
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            var sr = new StreamReader(resp.GetResponseStream());
            var pd = new List<PriceData>();

            if (sr == null)
                return pd;

            return ParseResponseStream(sr, lastKnownDate,symbol);
        }

        private List<PriceData> ParseResponseStream(StreamReader sr, DateTime? lastKnownDate, Symbol symbol)
        {
            List<PriceData> parsedData = new List<PriceData>();

            while (!sr.EndOfStream)
            {
                var newTicketData = ParseSingleLine(sr.ReadLine(), lastKnownDate, symbol);

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

                day.OpenPrice = Convert.ToDecimal(data[1].Replace("\"", "").Replace(".", ","), CultureInfo.GetCultureInfo("es")).TwoDecimalValues();
                day.MaxPrice = Convert.ToDecimal(data[2].Replace("\"", "").Replace(".", ","), CultureInfo.GetCultureInfo("es")).TwoDecimalValues();
                day.MinPrice = Convert.ToDecimal(data[3].Replace("\"", "").Replace(".", ","), CultureInfo.GetCultureInfo("es")).TwoDecimalValues();
                day.ClosingPrice = Convert.ToDecimal(data[4].Replace("\"", "").Replace(".", ","), CultureInfo.GetCultureInfo("es")).TwoDecimalValues();
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
    }
}
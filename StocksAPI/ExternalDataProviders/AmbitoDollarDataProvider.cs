using StocksAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace StocksAPI.ExternalDataProviders
{
    public class AmbitoDollarDataProvider : IDollarDataProvider
    {
        private const string BASE_URL_OFICIAL = "https://mercados.ambito.com/dolar/oficial/historico-general/{0}-{1}-{2}/{3}-{4}-{5}";
        private const string BASE_URL_CCL = "https://mercados.ambito.com//dolar/ccl/historico-general/{0}-{1}-{2}/{3}-{4}-{5}";

        public async Task<List<DollarData>> DownloadDollarData(DateTime lastKnownPrice)
        {
            lastKnownPrice = lastKnownPrice.AddDays(1);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(BASE_URL_OFICIAL, lastKnownPrice.Day,lastKnownPrice.Month,lastKnownPrice.Year,DateTime.Today.Day,DateTime.Today.Month,DateTime.Today.Year));
            WebResponse resp = await req.GetResponseAsync();

            var sr = new StreamReader(resp.GetResponseStream());
            var pd = new List<DollarData>();

            if (sr != null)
                pd.AddRange(ParseResponseStreamOficial(sr));

            req = (HttpWebRequest)WebRequest.Create(string.Format(BASE_URL_CCL, lastKnownPrice.Day, lastKnownPrice.Month, lastKnownPrice.Year, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year));
            resp = await req.GetResponseAsync();

            sr = new StreamReader(resp.GetResponseStream());

            if (sr != null)
                pd.AddRange(ParseResponseStreamCcl(sr));

            return pd;
        }

        private List<DollarData> ParseResponseStreamOficial(StreamReader sr)
        {
            var dollars = new List<DollarData>();

            JsonDocument resp = JsonDocument.Parse(sr.ReadToEnd());
            var root = resp.RootElement;

            for (int i = 1; i < root.GetArrayLength(); i++)
            {
                var line = root[i];
                dollars.Add(new DollarData() { ExchangeDate = DateTime.Parse(line[0].ToString(), CultureInfo.GetCultureInfo("ES")), Price = decimal.Parse(line[2].ToString(), CultureInfo.GetCultureInfo("ES")), DollarType = "OFICIAL" });
            }
            return dollars;
        }

        private List<DollarData> ParseResponseStreamCcl(StreamReader sr)
        {
            var dollars = new List<DollarData>();

            JsonDocument resp = JsonDocument.Parse(sr.ReadToEnd());
            var root = resp.RootElement;

            for (int i = 1; i < root.GetArrayLength(); i++)
            {
                var line = root[i];
                dollars.Add(new DollarData() { ExchangeDate = DateTime.Parse(line[0].ToString(),CultureInfo.GetCultureInfo("ES")), Price = decimal.Parse(line[1].ToString(),CultureInfo.GetCultureInfo("EN")), DollarType = "CCL" });
            }
            return dollars;
        }
    }
}
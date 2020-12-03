using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCoreVueStarter.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AspNetCoreVueStarter
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestGraph2
    {
        private const string URL = "https://api.applicationinsights.io/v1/apps/{0}/{1}/{2}?{3}";

        private const string URL2 = "https://api.applicationinsights.io/v1/apps/{0}/{1}?{2}";

        public static string GetTelemetry(string appid, string apikey,
        string queryType, string queryPath, string parameterString)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", apikey);
            var req = string.Format(URL, appid, queryType, queryPath, parameterString);
            HttpResponseMessage response = client.GetAsync(req).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }

        public static string GetOtherTelemetry(string appid, string apikey, string queryType, string queryPath, string parameterString)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", apikey);
            var req = string.Format(URL2, appid, queryPath, parameterString);
            HttpResponseMessage response = client.GetAsync(req).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }

        [HttpGet]
        public String Get()
        {

            string reply = GetOtherTelemetry("", "", "", "query", "query=exceptions | where timestamp > ago(24h) | summarize exceptioncount = count() by bin(timestamp, 1h), type | top 5 by exceptioncount desc");

            ReplyData parsed = JsonConvert.DeserializeObject<ReplyData>(reply);

            ChartData charted = new ChartData();
            charted.chartData = new List<DataPair>();

            foreach (List<object> item in parsed.tables.ElementAt(0).rows)
            {
                DataPair temp = new DataPair();
                temp.label = ((DateTime)item.ElementAt(0)).ToString();
                temp.hour = (DateTime)item.ElementAt(0);
                temp.except_type = (item.ElementAt(1)).ToString();
                temp.except_count = (long)item.ElementAt(2);
                charted.chartData.Add(temp);
            }

            charted.chartData = charted.chartData.OrderBy(c => c.hour).ToList();

            ChartDataWrapper sorted = new ChartDataWrapper();
            sorted.chartData = new PostSortData();
            sorted.chartData.labels = new List<string>();
            sorted.chartData.except_type = new List<string>();
            sorted.chartData.except_count = new List<long>();

            foreach (DataPair item in charted.chartData)
            {
                sorted.chartData.labels.Add(item.label);
                sorted.chartData.except_type.Add(item.except_type);
                sorted.chartData.except_count.Add(item.except_count);
            }

            string chartified = JsonConvert.SerializeObject(sorted);

            return chartified;
        }

        public static string getData()
        {
            return GetTelemetry("", "", "metrics", "requests/count", "timespan=P1D");
        }

    }
}
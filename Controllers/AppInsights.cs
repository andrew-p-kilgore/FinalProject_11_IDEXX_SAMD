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
    public class AppInsights
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
        public String Get([FromQuery] string name)
        {

            string reply = "";

            /*
             * The possible options for name are:
             *      All
             *      Regression Cornerstone Group
             *      Communicator Test
             *      QA CIS BATCH TEST
             *      Other Partners
            */
            // If the option selected is "All", then use data from all partners
            if (name.CompareTo("\"All\"") == 0)
            {
                reply = GetOtherTelemetry("","","", "query", "query=requests" +
                    "| where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\" | summarize avg(duration) | extend a = \"b\" | join" +
                    "(requests  | where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\" | summarize avg(duration) by bin(timestamp,1h) | extend  a = \"b\") on a" +
                    "| project timestamp, avg_duration1");
            }
            // If the option selected is "Other Partners", only use data from the groups not specified by name
            else if (name.CompareTo("\"Other Partners\"") == 0)
            {
                reply = GetOtherTelemetry("", "", "", "query", "query=requests" +
                "| where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\"" +
                " and customDimensions.GroupName != \"All\" and customDimensions.GroupName != \"Regression Cornerstone Group\"" +
                " and customDimensions.GroupName != \"Communicator Test\" and customDimensions.GroupName != \"QA CIS BATCH TEST\"" +
                "| summarize avg(duration) | extend a = \"b\" | join" +
                "(requests  | where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\"" +
                " and customDimensions.GroupName != \"All\" and customDimensions.GroupName != \"Regression Cornerstone Group\"" +
                " and customDimensions.GroupName != \"Communicator Test\" and customDimensions.GroupName != \"QA CIS BATCH TEST\"" +
                "| summarize avg(duration) by bin(timestamp,1h) | extend  a = \"b\") on a" +
                "| project timestamp, avg_duration1");
            }
            // If the option selected is not "All" nor "Other Partners", make a query for the specified partner group
            else
            {
                reply = GetOtherTelemetry("", "", "", "query", "query=requests" +
                    "| where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\" and customDimensions.GroupName == " + name + " " +
                    "| summarize avg(duration) | extend a = \"b\" | join" +
                    "(requests  | where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\" and customDimensions.GroupName == " + name + " " +
                    "| summarize avg(duration) by bin(timestamp,1h) | extend  a = \"b\") on a" +
                    "| project timestamp, avg_duration1");
            }

            ReplyData parsed = JsonConvert.DeserializeObject<ReplyData>(reply);

            ChartData charted = new ChartData();
            charted.chartData = new List<DataPair>();

            foreach (List<object> item in parsed.tables.ElementAt(0).rows)
            {
                DataPair temp = new DataPair();
                temp.hour = (DateTime)item.ElementAt(0);
                temp.label = ((DateTime)item.ElementAt(0)).ToString();
                temp.avg_duration = (double)item.ElementAt(1);
                charted.chartData.Add(temp);
            }

            charted.chartData = charted.chartData.OrderBy(c => c.hour).ToList();

            ChartDataWrapper sorted = new ChartDataWrapper();
            sorted.chartData = new PostSortData();
            sorted.chartData.labels = new List<string>();
            sorted.chartData.avg_duration = new List<double>();

            foreach (DataPair item in charted.chartData)
            {
                sorted.chartData.labels.Add(item.label);
                sorted.chartData.avg_duration.Add(item.avg_duration);
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
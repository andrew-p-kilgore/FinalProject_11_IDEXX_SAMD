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
    public class ResponseTypes
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
            Console.WriteLine("REQUEST: " + req);
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
            // Retrieve percentage of requests by resultCode over the past 24 hours
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
                reply = GetOtherTelemetry("", "", "", "query", "query=requests" +
                    "| where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\" | summarize count() | extend a = \"b\" | join" +
                    "(requests  | where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\" | summarize count() by resultCode | extend  a = \"b\") on a" +
                    "| project resultCode, percentage = (todouble(count_1) * 100 / todouble(count_))");
            }
            // If the option selected is "Other Partners", only use data from the groups not specified by name
            else if (name.CompareTo("\"Other Partners\"") == 0)
            {
                reply = GetOtherTelemetry("", "", "", "query", "query=requests" +
                "| where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\"" +
                " and customDimensions.GroupName != \"All\" and customDimensions.GroupName != \"Regression Cornerstone Group\"" +
                " and customDimensions.GroupName != \"Communicator Test\" and customDimensions.GroupName != \"QA CIS BATCH TEST\"" +
                "| summarize count() | extend a = \"b\" | join" +
                "(requests  | where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\"" +
                " and customDimensions.GroupName != \"All\" and customDimensions.GroupName != \"Regression Cornerstone Group\"" +
                " and customDimensions.GroupName != \"Communicator Test\" and customDimensions.GroupName != \"QA CIS BATCH TEST\"" +
                "| summarize count() by resultCode | extend  a = \"b\") on a" +
                "| project resultCode, percentage = (todouble(count_1) * 100 / todouble(count_))");
            }
            // If the option selected is not "All" nor "Other Partners", make a query for the specified partner group
            else
            {
                reply = GetOtherTelemetry("", "", "", "query", "query=requests" +
                    "| where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\" and customDimensions.GroupName == " + name + " " +
                    "| summarize count() | extend a = \"b\" | join" +
                    "(requests  | where timestamp > ago(1d) " + "and customDimensions contains \"GroupName\" and customDimensions.GroupName == " + name + " " +
                    "| summarize count() by resultCode | extend  a = \"b\") on a" +
                    "| project resultCode, percentage = (todouble(count_1) * 100 / todouble(count_))");
            }

            ReplyData parsed = JsonConvert.DeserializeObject<ReplyData>(reply);

            ChartData charted = new ChartData();
            charted.chartData = new List<DataPair>();
            int lessThan2 = 0;

            foreach (List<object> item in parsed.tables.ElementAt(0).rows)
            {
                DataPair temp = new DataPair();
                string itemLabel = (item.ElementAt(0)).ToString();

                // Remove "(see exception telemetries)" from the labels
                if (itemLabel.Contains("("))
                {
                    itemLabel = itemLabel.Substring(0, itemLabel.IndexOf("(") - 1);
                    itemLabel += "]";
                }

                temp.label = itemLabel;

                // If the percentage is an integer, convert the percentage to a double
                if (item.ElementAt(1) is Int64 @int)
                {
                    temp.percent_responses = @int * 1.0;
                }
                else
                {
                    temp.percent_responses = (double)item.ElementAt(1);
                }

                // Count the number of categories with a percentage lower than 1%
                if (temp.percent_responses < 1)
                    lessThan2++;

                charted.chartData.Add(temp);
            }

            charted.chartData = charted.chartData.OrderBy(c => c.label).ToList();

            ChartDataWrapper sorted = new ChartDataWrapper();
            sorted.chartData = new PostSortData();
            sorted.chartData.labels = new List<string>();
            sorted.chartData.percent_responses = new List<double>();

            // If there is more than one category with a percentage that is lower than 1%,
            // combine all categories with percentage lower than 1% into one category
            if (lessThan2 > 1)
            {
                DataPair otherPair = new DataPair();
                otherPair.label = "Other (";
                otherPair.percent_responses = 0;
                foreach (DataPair item in charted.chartData)
                {
                    double percent = item.percent_responses;

                    if (percent < 1)
                    {
                        if (otherPair.percent_responses > 0)
                        {
                            otherPair.label += ", ";
                        }
                        otherPair.label = otherPair.label + item.label;
                        otherPair.percent_responses += percent;
                    }
                    else
                    {
                        sorted.chartData.labels.Add(item.label);
                        sorted.chartData.percent_responses.Add(Math.Round(percent, 2));
                    }
                }
                otherPair.label += ")";
                sorted.chartData.labels.Add(otherPair.label);
                sorted.chartData.percent_responses.Add(Math.Round(otherPair.percent_responses, 2));
            }
            // When there is one or less categories with a percentage lower than 1%, leave the list alone
            else
            {
                foreach (DataPair item in charted.chartData)
                {
                    sorted.chartData.labels.Add(item.label);
                    sorted.chartData.percent_responses.Add(Math.Round(item.percent_responses, 2));
                }
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

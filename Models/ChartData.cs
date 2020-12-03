using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreVueStarter
{
    public class ChartData
    {
        public List<DataPair> chartData { get; set; }

    }
    public class DataPair
    {
        public string label { get; set; }
        public DateTime hour { get; set; }
        public double avg_duration { get; set; }
        public long except_count { get; set; }
        public string except_type { get; set; }
        public double percent_responses { get; set; }
    }
    public class ChartDataWrapper
    {
        public PostSortData chartData { get; set; }
    }
    public class PostSortData
    {
        public List<string> labels { get; set; }
        public List<double> avg_duration { get; set; }
        public List<long> except_count { get; set; }
        public List<string> except_type { get; set; }
        public List<double> percent_responses { get; set; }
    }
}

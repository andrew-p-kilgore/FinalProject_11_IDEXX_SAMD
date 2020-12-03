using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreVueStarter.Models
{
    public class ReplyData
    {
        public List<Table> tables { get; set; }
    }
    public class Table
    {
        public string name { get; set; }
        public List<Column> columns { get; set; }
        public List<List<object>> rows { get; set; }
    }
    public class Column
    {
        public string name { get; set; }
        public string type { get; set; }
    }
}

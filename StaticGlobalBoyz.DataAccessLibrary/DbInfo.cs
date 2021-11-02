using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.DataAccessLibrary
{
    public class DbInfo
    {
        public DbInfo(string dbName, string connStringName)
        {
            DbName = dbName;
            ConnStringName = connStringName;
        }
        public string DbName { get; set; }
        public string ConnStringName { get; set; }
    }
}

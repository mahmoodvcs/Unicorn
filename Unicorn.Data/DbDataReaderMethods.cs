using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Unicorn.Data
{
    public static class DbDataReaderMethods
    {
        public static string GetString2(this IDataReader reader, int index)
        {
            if (reader[index] == DBNull.Value || reader[index] == null)
                return "";
            return reader[index].ToString();
        }

        public static string GetString2(this IDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value || reader[name] == null)
                return "";
            return reader[name].ToString();
        }
    }
}

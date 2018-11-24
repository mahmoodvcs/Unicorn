using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Unicorn.Data
{
    public class BatchQueryExecutor
    {
        public static List<QueryResult> RunQuery(string query)
        {
            query = RemoveComments(query);
            string[] sepratore = { "GO" + Environment.NewLine, "go" + Environment.NewLine };
            string[] queries = query.Split(sepratore, StringSplitOptions.RemoveEmptyEntries);

            List<QueryResult> results = new List<QueryResult>();

            foreach (var q in queries)
            {
                QueryResult r = new QueryResult();
                r.Query = q;
                try
                {
                    var dt = SqlHelper.ExecuteCommand(query);
                    if (dt.Rows.Count == 1 && dt.Columns.Count == 1)
                        r.Result = Convert.ToInt32(dt.Rows[0][0]);
                    else
                        r.TableResult = dt;
                }
                catch(Exception ex)
                {
                    r.Exception = ex;
                }
                results.Add(r);
            }
            return results;
        }

        private static string RemoveComments(string batch_query)
        {
            batch_query = RemoveCommentsBetween(batch_query, "/*", "*/");
            batch_query = RemoveCommentsBetween(batch_query, "--", Environment.NewLine);

            return batch_query;
        }

        private static string RemoveCommentsBetween(string batch_query, string str1, string str2)
        {
            int i = 0;
            do
            {
                i = batch_query.IndexOf(str1);
                if (i == -1)
                    break;
                int j = batch_query.Substring(i).IndexOf(str2);
                batch_query = batch_query.Remove(i, j + 2);
            }
            while (true);
            return batch_query;
        }

    }

    public class QueryResult
    {
        public string Query { get; set; }
        public Exception Exception { get; set; }
        public int Result { get; set; }
        public DataTable TableResult { get; set; }
    }
}

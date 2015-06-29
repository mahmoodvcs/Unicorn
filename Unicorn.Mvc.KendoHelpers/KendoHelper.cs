using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kendo.Mvc.UI;
using Kendo.Mvc;
using System.ComponentModel;
using Unicorn.Data;
using Dapper;
using Kendo.Mvc.Infrastructure;
using System.Data.Common;
using System.Reflection;
using System.Data.SqlClient;
using Kendo.Mvc.UI.Fluent;
using System.Web.Mvc;

namespace Unicorn.Mvc.KendoHelpers
{
    public static class KendoHelper
    {
        public static DataSourceResult ToDataSourceResult<TModel>(this DataSourceRequest request,
            string fieldsList, string fromClaues, string where, string defaultOrder, object parameters, List<string> groupByColumns = null)
        {
            var orderBy = request.GetOrderBy();
            if (string.IsNullOrEmpty(orderBy))
                orderBy = defaultOrder;
            var where2 = request.GetWhere(groupByColumns);
            if (!string.IsNullOrEmpty(where2))
                if (!string.IsNullOrEmpty(where))
                    where = " (" + where2 + ") and " + where;
                else
                    where = where2;
            var sql = GetSql(request, fieldsList, fromClaues, where, orderBy);
            var conn = ConnectionManager.Instance.GetConnection();
            DataSourceResult res = new DataSourceResult();
            res.Data = Dapper.SqlMapper.Query<TModel>(conn, sql, parameters);
            sql = "select " + fieldsList + " from " + fromClaues;
            if (!string.IsNullOrEmpty(where))
                sql += " where " + where;

            List<Tuple<string, AggregateFunction>> agrs = new List<Tuple<string, AggregateFunction>>();
            foreach (var a in request.Aggregates)
            {
                foreach (var af in a.Aggregates)
                    agrs.Add(new Tuple<string, AggregateFunction>(af.AggregateMethodName + "([" + af.SourceField + "])", af));
            }
            var sql2 = "select count(*) _count ";
            if (agrs.Count > 0)
                sql2 += "," + string.Join(",", agrs.Select(aa => aa.Item1));
            sql2 += " from (" + sql + ") a";
            var dr = SqlHelper.ExecuteReader(sql2, GetSqlParameters(parameters));
            dr.Read();
            res.Total = (int)dr["_count"];
            List<AggregateResult> agres = new List<AggregateResult>();
            for (int i = 1; i < dr.FieldCount; i++)
            {
                agres.Add(new AggregateResult(dr[i], agrs[i - 1].Item2));
            }
            dr.Close();
            res.AggregateResults = agres;

            return res;
        }

        private static DbParameter[] GetSqlParameters(dynamic parameters)
        {
            List<DbParameter> pars = new List<DbParameter>();
            Type type = parameters.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var v = prop.GetValue(parameters);
                if (v != null)
                {
                    SqlParameter p = new SqlParameter("@" + prop.Name, v);
                    pars.Add(p);
                }
            }
            return pars.ToArray();
        }
        public static string GetSql(this DataSourceRequest request, string fieldsList, string fromClaues)
        {
            return GetSql(request, fieldsList, fromClaues, request.GetWhere(), request.GetOrderBy());
        }
        public static string GetSql(this DataSourceRequest request, string fieldsList, string fromClaues, string where, string orderBy)
        {
            return DBDataUtility.GetPagedSelectStatement(fromClaues, fieldsList, orderBy,
                where, (request.Page - 1) * request.PageSize, request.PageSize);
        }
        public static string GetOrderBy(this DataSourceRequest request)
        {
            return string.Join(",", request.Sorts.Select(s => s.Member +
                (s.SortDirection == ListSortDirection.Descending ? " desc" : "")));
        }
        public static string GetWhere(this DataSourceRequest request, List<string> groupByColumns=null)
        {
            return string.Join(" and ", request.Filters.Select(f => f.GetFilterPart()));
        }
        //public static GridDataOptions ToGridDataOptions(this DataSourceRequest request)
        //{
        //    GridDataOptions d = new GridDataOptions();
        //    d.PageIndex = request.Page;
        //    d.PageSize = request.PageSize;
        //    d.Sorting = new List<GridDataOptions.SortOptions>();
        //    if (request.Sorts != null)
        //        foreach (var s in request.Sorts)
        //            d.Sorting.Add(new GridDataOptions.SortOptions()
        //            {
        //                DataKey = s.Member,
        //                SortDirection = s.SortDirection == System.ComponentModel.ListSortDirection.Ascending ? "Asc" : "Desc"
        //            });
        //    d.Filtering = new List<GridDataOptions.FilterOptions>();
        //    //foreach (FilterDescriptor f in request.Filters)
        //    //    d.Filtering.Add(new GridDataOptions.FilterOptions() { DataKey = f.Member, FilterOperator = GetOperator(f.Operator), FilterValue = f.Value.ToString() });
        //    return d;
        //}
        private static string GetFilterPart(this IFilterDescriptor filter)
        {
            FilterDescriptor f;
            var filters = filter as CompositeFilterDescriptor;
            if (filters != null)
            {
                List<string> fs = new List<string>();
                foreach (var fd in filters.FilterDescriptors)
                    fs.Add(fd.GetFilterPart());
                return string.Format("({0})", string.Join(" " + filters.LogicalOperator.ToString() + " ", fs));
            }
            f = (FilterDescriptor)filter;
            return string.Format(f.GetFilterPartPattern(), f.Member, f.ConvertedValue);
        }
        private static string GetFilterPartPattern(this FilterDescriptor f)
        {
            string s = "";
            switch (f.Operator)
            {
                case FilterOperator.Contains:
                    return "{0} LIKE N'%{1}%'";
                case FilterOperator.DoesNotContain:
                    return "{0} NOT LIKE N'%{1}%'";
                case FilterOperator.EndsWith:
                    return "{0} LIKE N'%{1}'";
                case FilterOperator.StartsWith:
                    return "{0} LIKE N'{1}%'";
                    break;
                case FilterOperator.IsContainedIn:
                    //return "{0} LIKE N'%{1}%";
                    break;
                case FilterOperator.IsEqualTo:
                    s = "{0} = ";
                    break;
                case FilterOperator.IsGreaterThan:
                    s = "{0} > ";
                    break;
                case FilterOperator.IsGreaterThanOrEqualTo:
                    s = "{0} >= ";
                    break;
                case FilterOperator.IsLessThan:
                    s = "{0} < ";
                    break;
                case FilterOperator.IsLessThanOrEqualTo:
                    s = "{0} <= ";
                    break;
                case FilterOperator.IsNotEqualTo:
                    s = "{0} <> ";
                    break;
                default:
                    return "";
            }
            if (Type.GetTypeCode(f.MemberType) == TypeCode.String)
                s += "N'{1}'";
            else
                s += "{1}";
            return s;
        }

        public static void ReplaceName(this DataSourceRequest req, string oldName, string newName)
        {
            foreach (var f in req.Sorts)
            {
                if (f.Member == oldName)
                    f.Member = newName;
            }
        }
        public static void ReplaceNames(this DataSourceRequest req, Dictionary<string, string> map)
        {
            foreach (var key in map.Keys)
            {
                foreach (var f in req.Sorts)
                {
                    if (f.Member == key)
                        f.Member = map[key];
                }
                foreach (var f in req.Filters)
                {
                    foreach (var fd in f.GetFilterDescriptors())
                        if (fd.Member == key)
                            fd.Member = map[key];
                }
            }
        }

        static IEnumerable<FilterDescriptor> GetFilterDescriptors(this IFilterDescriptor filter)
        {
            List<FilterDescriptor> list = new List<FilterDescriptor>();
            if (filter is FilterDescriptor)
                list.Add((FilterDescriptor)filter);
            else if (filter is CompositeFilterDescriptor)
                foreach (var f in ((CompositeFilterDescriptor)filter).FilterDescriptors)
                {
                    list.AddRange(f.GetFilterDescriptors());
                }
            return list;
        }


        public static string RegisterKendoHelpersScript(this UrlHelper html)
        {
            return html.Content("~/Unicorn/Resource?a=Unicorn.Mvc.KendoHelpers&r=Unicorn.Mvc.KendoHelpers.js.kendoHelpers.js");
        }
    }
}
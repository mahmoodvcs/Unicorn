using Kendo.Mvc;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mvc.KendoHelpers
{
    public class GridFilterSqlGenerator
    {
        public GridFilterSqlGenerator(DataSourceRequest request)
        {
            this.request = request;
        }
        DataSourceRequest request;
        private static string GetFilterPart(IFilterDescriptor filter)
        {
            FilterDescriptor f;
            var filters = filter as CompositeFilterDescriptor;
            if (filters != null)
            {
                List<string> fs = new List<string>();
                foreach (var fd in filters.FilterDescriptors)
                    fs.Add(GetFilterPart(fd));
                return string.Format("({0})", string.Join(" " + filters.LogicalOperator.ToString() + " ", fs));
            }
            f = (FilterDescriptor)filter;
            return string.Format(GetFilterPartPattern(f), f.Member, f.ConvertedValue);
        }
        private static string GetFilterPartPattern(FilterDescriptor f)
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

        public static void ReplaceName(DataSourceRequest req, string oldName, string newName)
        {
            foreach (var f in req.Sorts)
            {
                if (f.Member == oldName)
                    f.Member = newName;
            }
        }
        public static void ReplaceNames(DataSourceRequest req, Dictionary<string, string> map)
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
                    foreach (var fd in GetFilterDescriptors(f))
                        if (fd.Member == key)
                            fd.Member = map[key];
                }
            }
        }

        static IEnumerable<FilterDescriptor> GetFilterDescriptors(IFilterDescriptor filter)
        {
            List<FilterDescriptor> list = new List<FilterDescriptor>();
            if (filter is FilterDescriptor)
                list.Add((FilterDescriptor)filter);
            else if (filter is CompositeFilterDescriptor)
                foreach (var f in ((CompositeFilterDescriptor)filter).FilterDescriptors)
                {
                    list.AddRange(GetFilterDescriptors(f));
                }
            return list;
        }

    }
}

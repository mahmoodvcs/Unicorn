using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mvc.KendoHelpers
{
    public static class KendoGridExtentions
    {
        public static GridBuilder<T> DoConfig<T>(this GridBuilder<T> g)  where T:class
        {
            g.Pageable(pg => pg.Localize()).Filterable(f => f.Localize());
            //g.DataSource(ds =>
            //{
            //    ds.
            //});
            return g;
        }
        public static void Localize(this PageableBuilder pg)
        {
            pg.Messages(msg => msg.Localize()).PageSizes(new int[] { 10, 20, 50, 100 }).Refresh(true);
        }
        public static void Localize(this PageableMessagesBuilder msg)
        {
            msg.First("").Last("").Previous("").Next("")
                .Display("سطر {0} تا {1} از {2} سطر".Localize()).Page("صفحه".Localize()).Of("از".Localize())
                .ItemsPerPage("تعداد سطرها در صفحه".Localize());
        }
        public static void Localize(this GridFilterableSettingsBuilder f)
        {
            f.Messages(m => m.Localize()).Operators(op => op.Localize());
        }
        public static void Localize(this FilterableOperatorsBuilder op)
        {
            op.ForNumber(fn=>fn.IsEqualTo("برایر با").IsGreaterThan("بزرگتر از").IsGreaterThanOrEqualTo("بزرگتر یا مساوی با")
                .IsLessThan("کوچکتر از").IsLessThanOrEqualTo("کوچکتر یا مساوی با").IsNotEqualTo("مخالف با")
                ).ForString(s=>
                s.Contains("شامل ... باشد").DoesNotContain("شامل ... نباشد").EndsWith("با ... خاتمه یابد")
                .IsEqualTo("برابر با").IsNotEqualTo("مخالف با").StartsWith("با ... شروع شود")
                );
        }
        public static void Localize(this FilterableMessagesBuilder msg)
        {
            msg.And("و").Cancel("لغو").Clear("حذف فیلتر").Filter("شرط").Info("")
                .Or("یا").SelectValue("انتخاب مقدار").Value("مقدار");
        }
        public static void AddCreateLocalize<T>(this GridToolBarCommandFactory<T> c) where T : class
        {
            c.Create().Text("جدید".Localize());
        }
        public static void Localize<T>(this GridActionCommandFactory<T> c, bool hasEdit = true, bool hasDelete = true) where T : class
        {
            if (hasEdit)
                c.Edit().Text("ویرایش".Localize()).UpdateText("ذخیره".Localize()).CancelText("لغو".Localize());
            if (hasDelete)
                c.Destroy().Text("حذف".Localize());
        }
        public static string Localize(this string s)
        {
            return s;
        }
 
        //public static GridTemplateColumnBuilder<TModel> BoundDate<TValue>(this GridColumnFactory<TModel> cols,
        //    Expression<Func<TModel, TValue>> expression)
        //{

        //}
    }
}

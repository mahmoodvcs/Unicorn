using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unicorn.Mvc.KendoHelpers
{
    public static class KendoGridExtentions
    {
        public delegate string TranslateDelegate(string s);
        public static TranslateDelegate Translate { get; set; } = KendoGridExtentions.NoTranslation;

        public static GridBuilder<T> DoConfig<T>(this GridBuilder<T> g) where T : class
        {
            g.Pageable(pg => pg.Localize()).Filterable(f => f.Localize());
            //g.DataSource(ds =>
            //{
            //    ds.
            //});
            return g;
        }
        public static GridBoundColumnBuilder<T> AsNumber<T>(this GridBoundColumnBuilder<T> col)
             where T : class
        {
            return col.ClientTemplate("<span class='number'>#=kendo.toString(" + col.Column.Member + ",'n0')#</span>");
        }
        public static GridBoundColumnBuilder<T> AddfooterAggregate<T>(this GridBoundColumnBuilder<T> col,string aggregate)
            where T : class
        {
            return col.ClientFooterTemplate("سود: <span class='number'>#=kendo.toString(" + aggregate + ",'n0')#</span>"); ;
        }
        public static PageableBuilder Localize(this PageableBuilder pg)
        {
            if (!Thread.CurrentThread.CurrentCulture.Name.StartsWith("fa"))
                return pg;
            return pg.Messages(msg => msg.Localize()).PageSizes(new int[] { 10, 20, 50, 100 }).Refresh(true);
        }
        public static GridPageableSettingsBuilder Localize(this GridPageableSettingsBuilder pg)
        {
            return pg.Messages(msg => msg.Localize()).PageSizes(new int[] { 10, 20, 50, 100 }).Refresh(true);
        }
        public static PageableMessagesBuilder Localize(this PageableMessagesBuilder msg)
        {
            return msg.First("").Last("").Previous("").Next("")
                .Display("سطر {0} تا {1} از {2} سطر".Localize()).Page("صفحه".Localize()).Of("از".Localize())
                .ItemsPerPage("تعداد سطرها در صفحه".Localize());
        }
        public static GridFilterableSettingsBuilder Localize(this GridFilterableSettingsBuilder f)
        {
            return f.Messages(m => m.Localize()).Operators(op => op.Localize());
        }
        public static FilterableOperatorsBuilder Localize(this FilterableOperatorsBuilder op)
        {
            if (!Thread.CurrentThread.CurrentCulture.Name.StartsWith("fa"))
                return op;
            op.ForNumber(fn => fn.IsEqualTo("برایر با").IsGreaterThan("بزرگتر از").IsGreaterThanOrEqualTo("بزرگتر یا مساوی با")
                .IsLessThan("کوچکتر از").IsLessThanOrEqualTo("کوچکتر یا مساوی با").IsNotEqualTo("مخالف با")
                ).ForString(s =>
                s.Contains("شامل ... باشد").DoesNotContain("شامل ... نباشد").EndsWith("با ... خاتمه یابد")
                .IsEqualTo("برابر با").IsNotEqualTo("مخالف با").StartsWith("با ... شروع شود")
                );
            return op;
        }
        public static void Localize(this FilterableMessagesBuilder msg)
        {
            if (!Thread.CurrentThread.CurrentCulture.Name.StartsWith("fa"))
                return;
            msg.And("و").Clear("حذف فیلتر").Filter("فیلتر").Info("")
                .Or("یا").SelectValue("انتخاب مقدار").Value("مقدار").IsTrue("بله").IsFalse("خیر");
        }
        public static void AddCreateLocalize<T>(this GridToolBarCommandFactory<T> c) where T : class
        {
            c.Create().Text("جدید".Localize());
        }
        public static GridActionCommandFactory<T> Localize<T>(this GridActionCommandFactory<T> c, bool hasEdit = true, bool hasDelete = true) where T : class
        {
            if (hasEdit)
                c.Edit().Text("ویرایش".Localize()).UpdateText("ذخیره".Localize()).CancelText("لغو".Localize());
            if (hasDelete)
                c.Destroy().Text("حذف".Localize());
            return c;
        }
        public static string Localize(this string s)
        {
            return Translate(s);
        }
        public static string NoTranslation(this string s)
        {
            return s;
        }

        //public static GridTemplateColumnBuilder<TModel> BoundDate<TValue>(this GridColumnFactory<TModel> cols,
        //    Expression<Func<TModel, TValue>> expression)
        //{

        //}
    }
}

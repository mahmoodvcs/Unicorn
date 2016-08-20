using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI.Html;
using Kendo.Mvc.UI.Fluent;
using Kendo.Mvc.UI;
using Unicorn.Mvc.KendoHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq.Expressions;
using Unicorn.Mvc;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Unicorn.Data.EF;
using Unicorn.Data.EF.DataAnnotations;
using System.Dynamic;

namespace Unicorn.Mvc.KendoHelpers
{
    public class DataEditorMemberMap//
    {
        //public DataEditorMemberMap(Expression<Func<TEntity, TMember>> member, string elementName)
        //{
        //    MemberName = member.MemberName();
        //    FormElement = elementName;
        //}
        public DataEditorMemberMap(string member, string elementName)
        {
            MemberName = member;
            FormElement = elementName;
        }
        public DataEditorMemberMap()
        {
        }
        public string MemberName { get; set; }
        public string FormElement { get; set; }
    }
    public class DataEditorOptions
    {

    }
    public static class DataEditorHelpers
    {

        public static GridBuilder<TEntity> EntityEditor<TContext, TEntity>(this HtmlHelper html, string name
            , params DataEditorMemberMap[] memberFormMaps)
            where TEntity : class
            where TContext : CachableDbContext, new()
        {
            var entityName = typeof(TEntity).Name;
            GridBuilder<TEntity> g = html.Kendo().Grid<TEntity>().Name(name);
            var props = typeof(TEntity).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Dictionary<PropertyInfo, Attribute[]> attrs = props.ToDictionary(p => p, p => p.GetCustomAttributes().ToArray());
            var keyProp = attrs.FirstOrDefault(pa => pa.Value.Any(a => a.GetType() == typeof(KeyAttribute)));
            List<string> dontRenderColumns = new List<string>();
            var fks = new List<Tuple<PropertyInfo, PropertyInfo>>();
            foreach (var p in attrs)
            {
                var fka = p.Key.GetCustomAttribute<ForeignKeyAttribute>();
                if (fka != null)
                {
                    var other = attrs.Keys.Single(k => k.Name == fka.Name);
                    if (Type.GetTypeCode(p.Key.PropertyType) == TypeCode.Int32)
                        fks.Add(new Tuple<PropertyInfo, PropertyInfo>(p.Key, other));
                    else
                        fks.Add(new Tuple<PropertyInfo, PropertyInfo>(other, p.Key));
                }

            }

            g.Columns(cols =>
                 {
                     foreach (var p in attrs)
                     {
                         if (p.Key == keyProp.Key)
                             continue;
                         //if (p.Value.Any(a => a is IgonreColumnAttribute))
                         //    continue;
                         if (memberFormMaps.Any(m => m.MemberName == p.Key.Name))
                             continue;
                         if (fks.Any(f => f.Item2 == p.Key))
                             continue;
                         var columnAttribute = p.Value.OfType<DataColumnAttribute>().FirstOrDefault();
                         if (columnAttribute != null && !columnAttribute.Display)
                             continue;
                         var fk = fks.SingleOrDefault(f => f.Item1 == p.Key);

                         if (fk != null)
                         {
                             var fkEntityType = fk.Item2.PropertyType;
                             var fkEntityId = GetIdField(fkEntityType);
                             var fkEntityNameField = GetEntityNameField(fkEntityType);
                             var data = new TContext().GetAll(fkEntityType).ToList();
                             //dynamic empty = new ExpandoObject();
                             //((IDictionary<string, object>)empty).Add(fkEntityId.Name, 0);
                             //((IDictionary<string, object>)empty).Add(fkEntityNameField.Name, "انتخاب کنید ...");
                             if (data.Any())
                             {
                                 var t = data[0].GetType();
                                 var first = t.GetConstructor(Type.EmptyTypes).Invoke(null);
                                data.Insert(0, first);
                             }
                             var fkCol = cols.ForeignKey(p.Key.Name, data, fkEntityId.Name, fkEntityNameField.Name);

                             SetColumnProperties(p.Key, p.Value, fkCol);
                             continue;
                         }
                         var col = cols.Bound(p.Key.Name);

                     }
                     cols.Command(c => c.Localize());
                 }).Editable(c => c.Mode(GridEditMode.InLine))
                .DataSource(ds =>
                {
                    var ds2 = ds.Ajax().Batch(false).Create(c => c.Action("Create", entityName).AddCRUDOperationData(memberFormMaps))
                        .Destroy("Delete", entityName)
                        .Read(r => r.Action("Get", entityName).AddCRUDOperationData(memberFormMaps))
                        .Update(r => r.Action("Update", entityName).AddCRUDOperationData(memberFormMaps))
                        //ds.Events(ev => ev.Error("onError"));
                        .Model(m =>
                        {
                            m.Id(keyProp.Key.Name);
                        });
                })
                .ToolBar(t => t.AddCreateLocalize())
                .Sortable().DoConfig();
            //.Events(ev => ev//.DataBound("dataBound")
            //    .Edit("createGridDatePicker"));

            return g;
        }

        //static GridColumnBuilderBase<TColumn, TColBuilder> CreateColumn<TColumn, TColBuilder>(PropertyInfo property)
        //    where TColumn : IGridColumn
        //    where TColBuilder : GridColumnBuilderBase<TColumn, TColBuilder>
        //{

        //}
        static void SetColumnProperties<TColumn, TColBuilder>(PropertyInfo property, Attribute[] attributes, GridColumnBuilderBase<TColumn, TColBuilder> column)
            where TColumn : class, IGridColumn
            where TColBuilder :GridColumnBuilderBase<TColumn, TColBuilder>
        {
            var columnAttribute = attributes.OfType<DataColumnAttribute>().FirstOrDefault();
            if( columnAttribute != null)
            {
                columnAttribute.Editable = columnAttribute.Editable;
            }
            var boundColumn = column as GridBoundColumnBuilder<TColumn>;
            var dataTypeAttr = attributes.OfType<DataTypeAttribute>().FirstOrDefault();
            if (boundColumn != null)
            {
                if (dataTypeAttr != null)
                {
                    switch (dataTypeAttr.DataType)
                    {
                        case DataType.Currency:
                            boundColumn = boundColumn.Format("{0:#,###}");
                            break;
                    }
                }

                if (columnAttribute != null)
                {
                    boundColumn.Sortable(columnAttribute.Sortable);
                }
                else
                {
                    boundColumn.Sortable(true);
                }
            }
        }
        private static PropertyInfo GetEntityNameField(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(pp => pp.PropertyType == typeof(string)).FirstOrDefault();
        }

        static PropertyInfo GetIdField(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(pp => pp.GetCustomAttribute<KeyAttribute>() != null).FirstOrDefault();
        }

        private static CrudOperationBuilder AddCRUDOperationData(this CrudOperationBuilder r, DataEditorMemberMap[] memberFormMaps)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var m in memberFormMaps)
            {
                sb.Append($"['{m.MemberName}','{m.FormElement}']");
            }
            if (sb.Length == 0)
                return r;
            return r.Data("(function(e){ DataEditorMemberMapGetData([" + sb.ToString()
                + "], e); })");
        }

        public static MvcHtmlString DataEditorScripts(this HtmlHelper html)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);
            StringBuilder sb = new StringBuilder();
            sb.Append("<script src='")
                    .Append(url.Content("~/Unicorn_Resource?a=UniDev.Web&r=UniDev.Web.js.DataEditor.js&"))
                    .Append(File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).Ticks.ToString())
                    .Append("'></script>\r\n");
            return new MvcHtmlString(sb.ToString());
        }

    }
}
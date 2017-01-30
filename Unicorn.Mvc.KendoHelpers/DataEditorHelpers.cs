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
            return EntityEditor<TContext, TEntity>(html, name, null, memberFormMaps);
        }
        public static GridBuilder<TEntity> EntityEditor<TContext, TEntity>(this HtmlHelper html, string name
            , object defaultFieldValues = null, params DataEditorMemberMap[] memberFormMaps)
            where TEntity : class
            where TContext : CachableDbContext, new()
        {
            var entityName = typeof(TEntity).Name;
            GridBuilder<TEntity> g = html.Kendo().Grid<TEntity>().Name(name);
            var props = typeof(TEntity).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Dictionary<PropertyInfo, Attribute[]> attrs = props.ToDictionary(p => p, p => p.GetCustomAttributes().ToArray());
            var keyProp = attrs.FirstOrDefault(pa => pa.Value.Any(a => a.GetType() == typeof(KeyAttribute)));
            List<string> dontRenderColumns = new List<string>();
            List<Tuple<PropertyInfo, PropertyInfo>> foreignKeys = GetForeignKeys(attrs);

            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            if (defaultFieldValues != null)
            {
                foreach(var p in defaultFieldValues.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    fieldValues[p.Name] = p.GetValue(defaultFieldValues);
            }

            List<PropertyInfo> usedFields = new List<PropertyInfo>();
            g.Columns(cols =>
            {
                foreach (var p in attrs)
                {
                    if (p.Key == keyProp.Key)
                        continue;
                    if (fieldValues.ContainsKey(p.Key.Name))
                        continue;
                    //if (p.Value.Any(a => a is IgonreColumnAttribute))
                    //    continue;
                    if (memberFormMaps.Any(m => m.MemberName == p.Key.Name))
                        continue;
                    if (foreignKeys.Any(f => f.Item2 == p.Key))
                        continue;
                    var columnAttribute = p.Value.OfType<DataColumnAttribute>().FirstOrDefault();
                    if (columnAttribute != null && !columnAttribute.Display)
                        continue;

                    usedFields.Add(p.Key);
                    var fk = foreignKeys.SingleOrDefault(f => f.Item1 == p.Key);

                    if (fk != null)
                    {
                        CreateForeignKeyColumn<TContext, TEntity>(cols, p, fk);
                        continue;
                    }
                    if (CheckAndAddComboBoxColumn<TContext, TEntity>(cols, p))
                        continue;
                    if(p.Key.PropertyType.IsEnum)
                    {
                        CreateEnumColumn<TContext, TEntity>(cols, p);
                        continue;
                    }
                    var col = cols.Bound(p.Key.Name);

                }
                cols.Command(c => c.Localize());
            }).Editable(c => c.Mode(GridEditMode.InLine))
                .DataSource(ds =>
                {
                    var ds2 = ds.Ajax().Batch(false).Create(c => c.Action("Create", entityName).AddCRUDOperationData(fieldValues, memberFormMaps))
                        .Destroy("Delete", entityName)
                        .Read(r => r.Action("Get", entityName).AddCRUDOperationData(fieldValues, memberFormMaps))
                        .Update(r => r.Action("Update", entityName).AddCRUDOperationData(fieldValues, memberFormMaps))
                        //ds.Events(ev => ev.Error("onError"));
                        .Model(m =>
                        {
                            m.Id(keyProp.Key.Name);
                            foreach (var f in usedFields)
                            {
                                m.Field(f.Name, f.PropertyType);
                            }
                            foreach (var p in props)
                            {
                                if(!usedFields.Any(f=>f.Name == p.Name))
                                    m.Field(p.Name, p.PropertyType).Editable(false);
                            }
                        });
                })
                .ToolBar(t => t.AddCreateLocalize())
                .Sortable().DoConfig();
            //.Events(ev => ev//.DataBound("dataBound")
            //    .Edit("createGridDatePicker"));

            return g;
        }

        private static bool CheckAndAddComboBoxColumn<TContext, TEntity>(GridColumnFactory<TEntity> cols, KeyValuePair<PropertyInfo, Attribute[]> p)
            where TContext : CachableDbContext, new()
            where TEntity : class
        {
            var comboAttr = p.Value.FirstOrDefault(a => a.GetType() == typeof(ComboBoxColumnAttribute)) as ComboBoxColumnAttribute;
            if (comboAttr == null)
                return false;
            var fkEntityType = comboAttr.ReferenceType;
            CreateForeignKeyColumn<TContext, TEntity>(cols, fkEntityType, p);
            return true;
        }

        private static void CreateForeignKeyColumn<TContext, TEntity>(GridColumnFactory<TEntity> cols, Type fkEntityType, KeyValuePair<PropertyInfo, Attribute[]> p)
            where TContext : CachableDbContext, new()
            where TEntity : class
        {
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
        }

        private static void CreateForeignKeyColumn<TContext, TEntity>(GridColumnFactory<TEntity> cols, KeyValuePair<PropertyInfo, Attribute[]> p, Tuple<PropertyInfo, PropertyInfo> fk)
            where TContext : CachableDbContext, new()
            where TEntity : class
        {
            var fkEntityType = fk.Item2.PropertyType;
            CreateForeignKeyColumn<TContext, TEntity>(cols, fkEntityType, p);
        }
        static void CreateEnumColumn<TContext, TEntity>(GridColumnFactory<TEntity> cols, KeyValuePair<PropertyInfo, Attribute[]> p) where TContext : CachableDbContext, new()
                    where TEntity : class
        {
            List<KeyValuePair<int, string>> data = new List<KeyValuePair<int, string>>();
            var values = Enum.GetValues(p.Key.PropertyType);
            foreach (var v in values)
            {
                var name = Enum.GetName(p.Key.PropertyType, v);
                var title = TitleAttribute.GetTitle(p.Key.PropertyType, name);
                if (string.IsNullOrEmpty(title))
                    title = name;
                data.Add(new KeyValuePair<int, string>((int)v, title));
            }
            var col = cols.ForeignKey(p.Key.Name, data, "Key", "Value");
            SetColumnProperties(p.Key, p.Value, col);
        }

        private static List<Tuple<PropertyInfo, PropertyInfo>> GetForeignKeys(Dictionary<PropertyInfo, Attribute[]> attrs)
        {
            var fks = new List<Tuple<PropertyInfo, PropertyInfo>>();
            foreach (var p in attrs)
            {
                var fka = p.Value.FirstOrDefault(a => a.GetType() == typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;
                //Key.GetCustomAttribute<ForeignKeyAttribute>();
                if (fka != null)
                {
                    var other = attrs.Keys.Single(k => k.Name == fka.Name);
                    var typeCode = Type.GetTypeCode(p.Key.PropertyType);
                    if (typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64)
                        fks.Add(new Tuple<PropertyInfo, PropertyInfo>(p.Key, other));
                    else
                        fks.Add(new Tuple<PropertyInfo, PropertyInfo>(other, p.Key));
                }
                //var comboAttr = p.Value.FirstOrDefault(a => a.GetType() == typeof(ComboBoxColumnAttribute));
                //if (comboAttr != null)
                //{
                //    var other = attrs.Keys.Single(k => k.Name == fka.Name);
                //    var typeCode = Type.GetTypeCode(p.Key.PropertyType);
                //    if (typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64)
                //        fks.Add(new Tuple<PropertyInfo, PropertyInfo>(p.Key, other));
                //    else
                //        fks.Add(new Tuple<PropertyInfo, PropertyInfo>(other, p.Key));
                //}
            }

            return fks;
        }

        private static bool CanBeKey(Type t)
        {
            var code = Type.GetTypeCode(t);
            return (code == TypeCode.Int16 || code == TypeCode.Int32 || code == TypeCode.Int64 ||
                code == TypeCode.UInt16 || code == TypeCode.UInt32 || code == TypeCode.UInt64 ||
                code == TypeCode.String || t == typeof(Guid));
        }

        //static GridColumnBuilderBase<TColumn, TColBuilder> CreateColumn<TColumn, TColBuilder>(PropertyInfo property)
        //    where TColumn : IGridColumn
        //    where TColBuilder : GridColumnBuilderBase<TColumn, TColBuilder>
        //{

        //}
        static void SetColumnProperties<TColumn, TColBuilder>(PropertyInfo property, Attribute[] attributes, GridColumnBuilderBase<TColumn, TColBuilder> column)
            where TColumn : class, IGridColumn
            where TColBuilder : GridColumnBuilderBase<TColumn, TColBuilder>
        {
            var columnAttribute = attributes.OfType<DataColumnAttribute>().FirstOrDefault();
            if (columnAttribute != null)
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
            return AddCRUDOperationData(r, null, memberFormMaps);
        }
        private static CrudOperationBuilder AddCRUDOperationData(this CrudOperationBuilder r, Dictionary<string, object> fieldValues, DataEditorMemberMap[] memberFormMaps)
        {
            StringBuilder sb = new StringBuilder("function(e){");
            foreach (var k in fieldValues.Keys)
            {
                sb.Append("e['").Append(k).Append("']=").Append(Newtonsoft.Json.JsonConvert.SerializeObject(fieldValues[k])).Append(";");
            }
            sb.Append(" return null;}");
            return r.Data(sb.ToString());
            //return r.Data("{ return " + Newtonsoft.Json.JsonConvert.SerializeObject(fieldValues) + "; })");
            //foreach (var m in memberFormMaps)
            //{
            //    sb.Append($"['{m.MemberName}','{m.FormElement}']");
            //}
            //if (sb.Length == 0)
            //    return r;
            //return r.Data("(function(e){ DataEditorMemberMapGetData([" + sb.ToString()
            //    + "], e); })");
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
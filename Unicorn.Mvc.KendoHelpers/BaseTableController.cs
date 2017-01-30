using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Text;
using System.Reflection;
using Unicorn.Data.EF;
using Unicorn.Data.EF.DataAnnotations;
using Kendo.Mvc;

namespace Unicorn.Mvc.KendoHelpers
{
    public abstract class BaseTableController<TContext, T> : Unicorn.Mvc.Controllers.UniControllerBase
        where T : class
        where TContext : DbContext, new()
    {
        TContext db = new TContext();
        protected TContext Context => db;

        public event EventHandler<EntityEventArgs<T>> EntitySaving;
        public event EventHandler<EntityEventArgs<T>> EntitySaved;

        public BaseTableController()
        {
            JsonDateConvertSetting = Mvc.JsonDateConvertSetting.PersianDate;
        }
        public virtual ActionResult Index()
        {
            return View(typeof(T).Name);
        }
        public virtual JsonResult Get([DataSourceRequest]DataSourceRequest request)
        {
            ReplaceNames(request);
            List<IFilterDescriptor> filters = new List<IFilterDescriptor>();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!string.IsNullOrEmpty(Request.Params[p.Name]))
                    filters.Add(new FilterDescriptor(p.Name, FilterOperator.IsEqualTo, Request.Params[p.Name]));
            }
            return Json(Context.Set<T>().Where(filters)
                .ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        private void ReplaceNames(DataSourceRequest request)
        {
            var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Select(p => new { p.Name, DataAttribute = p.GetCustomAttribute<DataColumnAttribute>() })
                .Where(p => p.DataAttribute != null && !string.IsNullOrEmpty(p.DataAttribute.MappedFromColumn))
                .ToDictionary(p => p.Name, p=>p.DataAttribute.MappedFromColumn);
            request.ReplaceNames(props);
        }

        public virtual JsonResult Create(T v, [DataSourceRequest]DataSourceRequest request)
        {
            EntitySaving?.Invoke(this, new EntityEventArgs<T>(v, EntityState.Added));
            Context.Set<T>().Add(v);
            Context.SaveChanges();
            EntitySaved?.Invoke(this, new EntityEventArgs<T>(v, EntityState.Added));
            return Json(new[] { v }.ToDataSourceResult(request));
        }
        public virtual JsonResult Update(T p, [DataSourceRequest]DataSourceRequest request)
        {
            Context.Set<T>().Attach(p);
            Context.Entry(p).State = System.Data.Entity.EntityState.Modified;
            EntitySaving?.Invoke(this, new EntityEventArgs<T>(p, EntityState.Modified));
            Context.SaveChanges();
            EntitySaved?.Invoke(this, new EntityEventArgs<T>(p, EntityState.Modified));
            return Json(new[] { p }.ToDataSourceResult(request));
        }
        public virtual JsonResult Delete(T v, [DataSourceRequest]DataSourceRequest request)
        {
            Context.Set<T>().Attach(v);
            EntitySaving?.Invoke(this, new EntityEventArgs<T>(v, EntityState.Deleted));
            Context.Set<T>().Remove(v);
            Context.SaveChanges();
            EntitySaved?.Invoke(this, new EntityEventArgs<T>(v, EntityState.Deleted));
            return Json(new[] { v }.ToDataSourceResult(request));
        }

        //public JsonDateConvertSetting JsonDateConvertSetting { get; set; }
        protected override JsonResult Json(object data, string contentType,
            Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult(JsonDateConvertSetting)
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Text;

namespace Unicorn.Mvc.KendoHelpers
{
    public abstract class BaseTableController<TContext, T> : Unicorn.Mvc.Controllers.UniControllerBase
        where T : class
        where TContext : DbContext, new()
    {
        protected TContext db = new TContext();
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
            return Json(db.Set<T>()
                .ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult Create(T v, [DataSourceRequest]DataSourceRequest request)
        {
            db.Set<T>().Add(v);
            db.SaveChanges();
            return Json(new[] { v }.ToDataSourceResult(request));
        }
        public virtual JsonResult Update(T p, [DataSourceRequest]DataSourceRequest request)
        {
            db.Set<T>().Attach(p);
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new[] { p }.ToDataSourceResult(request));
        }
        public virtual JsonResult Delete(T v, [DataSourceRequest]DataSourceRequest request)
        {
            db.Set<T>().Attach(v);
            db.Set<T>().Remove(v);
            db.SaveChanges();
            return Json(new[] { v }.ToDataSourceResult(request));
        }

        public JsonDateConvertSetting JsonDateConvertSetting { get; set; }
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
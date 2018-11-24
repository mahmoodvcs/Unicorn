using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Unicorn.Mvc.Controllers
{
    public abstract class UniControllerBase : Controller
    {
        protected UniControllerBase()
        {
            JsonResultSettings = new JsonResultSettings
            {
                DateConvertSetting = JsonDateConvertSetting.PersianDate,
                UseCamelCaseNames = false
            };
            //logger = DependencyResolver.Current.GetService<ILogger>();// new SqlLogger();
            //traceLog = NLog.LogManager.GetCurrentClassLogger();
            UseNativeJsonSerializer = false;
        }
        public bool UseNativeJsonSerializer { get; set; }
        //public ControllerBase(ILogger logger)
        //{
        //    JsonDateConvertSetting = JsonDateConvertSetting.PersianDate;
        //    this.logger = logger;
        //    traceLog = NLog.LogManager.GetCurrentClassLogger();
        //}
        //protected readonly ILogger logger;
        //protected readonly NLog.Logger traceLog;

        //public JsonDateConvertSetting JsonDateConvertSetting { get; set; }
        //public bool UseCamelCaseJsonNames { get; set; } = false;

        public JsonResultSettings JsonResultSettings { get; set; }

        public string UserName
        {
            get { return User == null || !User.Identity.IsAuthenticated ? null : User.Identity.Name; }
        }
        public virtual JsonResult JsonContract(object data)
        {
            return new JsonDataContractActionResult(data);
        }

        //public void LogException(Exception ex)
        //{
        //    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //}
        protected override JsonResult Json(object data, string contentType,
            Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (UseNativeJsonSerializer)
                return base.Json(data, contentType, contentEncoding, behavior);
            else
                return new JsonNetResult(JsonResultSettings)
                {
                    Data = data,
                    ContentType = contentType,
                    ContentEncoding = contentEncoding,
                    JsonRequestBehavior = behavior,
                };
        }

        public JsonNetResult Json(object data,
            params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Json(data, JsonRequestBehavior.DenyGet, converters);
        }
        public JsonNetResult Json(object data, JsonRequestBehavior behavior,
            params Newtonsoft.Json.JsonConverter[] converters)

        {
            var result = (JsonNetResult)Json(data, "application/json", Encoding.UTF8, behavior);
            foreach (var c in converters)
            {
                result.Settings.Converters.Add(c);
            }
            return result;
        }
        //public virtual JsonResult Error(Exception ex)
        //{
        //    LogException(ex);
        //    return Error(ex.Message);
        //}
        public virtual JsonResult Error(string msg)
        {
            return Json(new { ok = false, message = msg }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult OK()
        {
            return Json(new { ok = true });
        }
        public void ShowMessage(string msg, MessageType type = MessageType.Info, bool closable = true)
        {
            UIMessage m = new UIMessage()
            {
                Message = msg,
                Type = type,
                Closable = closable
            };
            //ViewBag.Message = m;
            TempData["Unicorn.Message"] = m;
        }
        protected bool ViewExists(string name)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, name, null);
            return (result.View != null);
        }
    }
}

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
            //logger = DependencyResolver.Current.GetService<ILogger>();// new SqlLogger();
            JsonDateConvertSetting = JsonDateConvertSetting.PersianDate;
            //traceLog = NLog.LogManager.GetCurrentClassLogger();
        }
        //public ControllerBase(ILogger logger)
        //{
        //    JsonDateConvertSetting = JsonDateConvertSetting.PersianDate;
        //    this.logger = logger;
        //    traceLog = NLog.LogManager.GetCurrentClassLogger();
        //}
        //protected readonly ILogger logger;
        //protected readonly NLog.Logger traceLog;
        public JsonDateConvertSetting JsonDateConvertSetting { get; set; }
        public string UserName
        {
            get { return User == null ? null : User.Identity.Name; }
        }
        public virtual JsonResult JsonContract(Object data)
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
            return new JsonNetResult(JsonDateConvertSetting)
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        //public virtual JsonResult Error(Exception ex)
        //{
        //    LogException(ex);
        //    return Error(ex.Message);
        //}
        public virtual JsonResult Error(string msg)
        {
            return Json(new { ok = false, message = msg });
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

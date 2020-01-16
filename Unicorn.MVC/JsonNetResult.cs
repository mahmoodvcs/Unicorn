using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Unicorn.Mvc.JsonConverters;

namespace Unicorn.Mvc
{
    public enum JsonDateConvertSetting
    {
        None,
        PersianDate,
        PersianDateTime,
        Javascript
    }
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
        }
        public JsonNetResult(JsonSerializerSettings settings)
        {
            this.Settings = settings;
        }
        public JsonNetResult(JsonResultSettings setting)
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter>()
            };
            if (setting.DateConvertSetting != JsonDateConvertSetting.None)
            {
                if (setting.DateConvertSetting == JsonDateConvertSetting.Javascript)
                {
                    Settings.Converters.Add(new JsDateTimeConverter());
                }
                else
                {
                    Settings.Converters.Add(new JsonPersianDateTimeConverter(setting.DateConvertSetting == JsonDateConvertSetting.PersianDateTime, false));
                }
            }
            foreach (var c in setting.CustomConverters)
            {
                Settings.Converters.Add(c);
            }
            if (setting.UseCamelCaseNames)
                Settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            try
            {
                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

                if (this.ContentEncoding != null)
                    response.ContentEncoding = this.ContentEncoding;
                if (this.Data == null)
                    return;

                var scriptSerializer = JsonSerializer.Create(this.Settings);

                using (var sw = new StringWriter())
                {
                    scriptSerializer.Serialize(sw, this.Data);
                    response.Write(sw.ToString());
                }
            }
            catch
            {
                base.ExecuteResult(context);
            }
        }
    }
}
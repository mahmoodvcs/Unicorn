using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Unicorn.Mvc
{
    public enum JsonDateConvertSetting
    {
        None,
        PersianDate,
        PersianDateTime
    }
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
            : this(JsonDateConvertSetting.PersianDate)
        {

        }
        public JsonNetResult(JsonSerializerSettings settings)
        {
            this.Settings = settings;
        }
        public JsonNetResult(JsonDateConvertSetting dateSetting)
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter>()
            };
            if (dateSetting != JsonDateConvertSetting.None)
                Settings.Converters.Add(new JsonPersianDateTimeConverter(dateSetting == JsonDateConvertSetting.PersianDateTime, false));
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
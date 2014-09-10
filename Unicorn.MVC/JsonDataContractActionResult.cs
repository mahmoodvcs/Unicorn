using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace Unicorn.MVC
{
    public class JsonDataContractActionResult : JsonResult
    {
        public JsonDataContractActionResult(Object data)
        {
            this.Data = data;
        }

        public Object Data { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(JsonSerializeDataContract(this.Data));
        }
        public static string JsonSerializeDataContract(object obj)
        {
            var serializer = new DataContractJsonSerializer(obj.GetType(), new Type[] { typeof(JsonDictionary<string, object>) });
            String output = String.Empty;
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                output = Encoding.UTF8.GetString(ms.ToArray());
            }
            return output;
        }
    }
}
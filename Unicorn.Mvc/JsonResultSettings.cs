using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicorn.Mvc.JsonConverters;

namespace Unicorn.Mvc
{
    public class JsonResultSettings
    {
        public JsonDateConvertSetting DateConvertSetting { get; set; } = JsonDateConvertSetting.PersianDate;
        public bool UseCamelCaseNames { get; set; } = false;
        public List<JsonConverter> CustomConverters { get; set; } = new List<JsonConverter>();
        //public PropertyRenameAndIgnoreSerializerContractResolver ContractResolver { get; private set; }
        //public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        //{
        //    if (ContractResolver == null)
        //    {
        //        ContractResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
        //    }
        //    ContractResolver.IgnoreProperty(type, jsonPropertyNames);
        //}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mvc
{
    public class JsonResultSettings
    {
        public JsonDateConvertSetting DateConvertSetting { get; set; } = JsonDateConvertSetting.PersianDate;
        public bool UseCamelCaseNames { get; set; } = false;
    }
}

using System;
using System.Collections.Generic;

using System.Text;

namespace Unicorn.Web.UI
{
    public interface ISecurityControl
    {
        string SecurityKey { get; set; }   
    }
    public interface IDataControl
    {
        string FieldName { get; set; }
        object Value { get; set; }
    }
}

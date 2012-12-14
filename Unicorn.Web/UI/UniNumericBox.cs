using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Web.UI;
using System.ComponentModel;

namespace Unicorn.Web.UI
{
    public class UniNumericBox:RadNumericTextBox,IDataControl
    {
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }

        object IDataControl.Value
        {
            get { return Value; }
            set {
                if (value == null)
                    Value = null;
                else
                    switch (System.Type.GetTypeCode(value.GetType()))
                    {
                        case TypeCode.Double:
                            Value = (double)value;
                            break;
                        case TypeCode.Single:
                            Value = (float)value;
                            break;
                        case TypeCode.Decimal:
                            Value = (double)(decimal)value;
                            break;
                        case TypeCode.Int32:
                            Value = (int)value;
                            break;
                        case TypeCode.Int64:
                            Value = (long)value;
                            break;
                        case TypeCode.SByte:
                            Value = (sbyte)value;
                            break;
                        case TypeCode.Byte:
                            Value = (byte)value;
                            break;
                        case TypeCode.Int16:
                            Value = (short)value;
                            break;
                        //case TypeCode.UInt16:
                        //    Value = (ushort)value;
                        //    break;
                        //case TypeCode.UInt32:
                        //    Value = (uint)value;
                        //    break;
                        //case TypeCode.UInt64:
                        //    Value = (ulong)value;
                        //    break;
                        default:
                            Value = double.Parse(value.ToString());
                            break;
                    }
            }
        }
    }
}

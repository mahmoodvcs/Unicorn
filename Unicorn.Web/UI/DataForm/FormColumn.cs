using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Jco.Web.Controls
{
    public class FormColumn
    {
        private string dataField;
        [Category("Data")]
        [Localizable(false)]
        [DefaultValue("")]
        public string DataField
        {
            get
            {
                return dataField;
            }
            set
            {
                dataField = value;
            }
        }
    }
}

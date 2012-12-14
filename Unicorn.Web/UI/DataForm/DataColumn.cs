using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Unicorn.Web.UI
{
    public class DataColumn
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

        [Category("Apearance")]
        [Localizable(true)]
        [DefaultValue("")]
        public string HeaderText { get; set; }
    }
}

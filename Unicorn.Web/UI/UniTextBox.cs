using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Unicorn.Web.UI
{
    public class UniTextBox : TextBox, IDataControl, ISecurityControl
    {
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }

        public object Value
        {
            get { return Text; }
            set { Text = (string)(value ?? ""); }
        }
    }
    public class UniPanel : Panel, ISecurityControl
    {
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }
    }
    public class UniButton : Button, ISecurityControl
    {
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }
    }
    public class UniDropDownList : DropDownList, IDataControl, ISecurityControl
    {
        [Bindable(true)]
        [Category("Behaviour")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }

        public object Value
        {
            get { return SelectedValue; }
            set
            {
                if (value == null)
                {
                    SelectedValue = null;
                    SelectedIndex = -1;
                }
                else
                    SelectedValue = value.ToString();
            }
        }
    }
    public class UniCheckBox : CheckBox, IDataControl, ISecurityControl
    {
        [Bindable(true)]
        [Category("Behaviour")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }

        public object Value
        {
            get { return Checked; }
            set { Checked = (bool)(value ?? false); }
        }
    }
    public class UniCheckBoxList : CheckBoxList, ISecurityControl
    {
        //[Bindable(true)]
        //[Category("Behaviour")]
        //public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }


    }
    public class UniRadioButton : RadioButton, IDataControl, ISecurityControl
    {
        [Bindable(true)]
        [Category("Behaviour")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }

        public object Value
        {
            get { return Checked; }
            set { Checked = (bool)(value ?? false); }
        }
    }

    public class UniLabel : Label, ISecurityControl, IDataControl
    {
        [Bindable(true)]
        [Category("Behaviour")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }

        public object Value
        {
            get { return Text; }
            set { Text = (string)(value ?? ""); }
        }
    }
}

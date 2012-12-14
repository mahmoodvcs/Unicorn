using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Collections;

namespace Unicorn.Web.UI
{
    [DefaultProperty("PersianDate")]
    [ToolboxData("<{0}:UniDatePicker runat=server></{0}:UniDatePicker>")]
    [ValidationProperty("PersianDate")]
    public class UniDatePicker : CompositeControl, IDataControl, ISecurityControl
    {
        public UniDatePicker()
        {
            // textRead = false;
            BindingProperty = DatePickerBindingProperty.Text;
        }
        TextBox textBox;
        //RequiredFieldValidator requiredValidator;
        //RangeValidator rangeValidator;
        MKB.TimePicker.TimeSelector timePicker;
        //bool textRead;

        #region ValueChanged

        public event EventHandler ValueChanged;
        /// <summary>
        /// Triggers the ValueChanged event.
        /// </summary>
        public virtual void OnValueChanged(EventArgs ea)
        {
            if (ValueChanged != null)
                ValueChanged(this, ea);
        }

        public bool AutoPostBack
        {
            get
            {
                EnsureChildControls();
                return textBox.AutoPostBack;
            }
            set
            {
                EnsureChildControls();
                textBox.AutoPostBack = value;
            }
        }
        #endregion

        #region Properties

        [Bindable(true)]
        [Category("Behaviour")]
        [Localizable(true)]
        public DateTime? Date
        {
            get
            {
                ReadText();
                return ((ViewState["Date"] == null) ? DateTime.Now : (DateTime)((PersianDateTime)ViewState["Date"]));
            }
            set
            {
                if (value == null)
                    ViewState.Remove("Date");
                else
                    ViewState["Date"] = (PersianDateTime)value;
                OnValueChanged(EventArgs.Empty);
                //textRead = true;
                SetText();
            }
        }

        [Bindable(true)]
        [Category("Behaviour")]
        [Localizable(true)]
        public PersianDateTime PersianDate
        {
            get
            {
                ReadText();
                return ((ViewState["Date"] == null) ? (PersianDateTime)DateTime.Now : (PersianDateTime)ViewState["Date"]);
            }
            set
            {
                ViewState["Date"] = value;
                OnValueChanged(EventArgs.Empty);
                SetText();
                //textRead = true;
            }
        }

        [Category("Behaviour")]
        [DefaultValue(DatePickerMode.Date)]
        public DatePickerMode Mode
        {
            get
            {
                return ((ViewState["Mode"] == null) ? DatePickerMode.Date : (DatePickerMode)ViewState["Mode"]);
            }
            set
            {
                ViewState["Mode"] = value;
                EnsureChildControls();
                timePicker.Visible = Mode == DatePickerMode.Time || Mode == DatePickerMode.DateTime;
            }
        }
        [Category("Behaviour")]
        [DefaultValue("")]
        public string RangeValidationText
        {
            get
            {
                return ((ViewState["RangeValidationText"] == null) ? "" : (string)ViewState["RangeValidationText"]);
            }
            set
            {
                ViewState["RangeValidationText"] = value;
            }
        }
        [Bindable(true)]
        [Category("Behaviour")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                ReadText();
                if (ViewState["Date"] == null)
                    return "";
                switch (Mode)
                {
                    case DatePickerMode.Date:
                        return PersianDate.ToShortDateString();
                    case DatePickerMode.Time:
                        return PersianDate.ToShortTimeString();
                    default:
                        return PersianDate.ToString();
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ViewState["Date"] = null;
                else
                    PersianDate = PersianDateTime.Parse(value);
                OnValueChanged(EventArgs.Empty);
                //textRead = true;
            }
        }

        [Category("Behaviour")]
        [DefaultValue(false)]
        [Bindable(true)]
        public bool ReverseDate
        {
            get
            {
                return ((ViewState["ReverseDate"] == null) ? false : (bool)ViewState["ReverseDate"]);
            }
            set
            {
                ViewState["ReverseDate"] = value;
            }
        }

        [Category("Behaviour")]
        [DefaultValue(false)]
        [Bindable(true)]
        public bool ReadOnly
        {
            get
            {
                return ((ViewState["ReadOnly"] == null) ? false : (bool)ViewState["ReadOnly"]);
            }
            set
            {
                ViewState["ReadOnly"] = value;
            }
        }

        //[Category("Behaviour")]
        //[DefaultValue(false)]
        //[Bindable(true)]
        //public bool EnableRequiredValidation
        //{
        //    get
        //    {
        //        return ((ViewState["RequiredValidation"] == null) ? false : (bool)ViewState["RequiredValidation"]);
        //    }
        //    set
        //    {
        //        ViewState["RequiredValidation"] = value;
        //    }
        //}
        //[Category("Behaviour")]
        //[DefaultValue(null)]
        //[Bindable(true)]
        //public PersianDateTime MinDate
        //{
        //    get
        //    {
        //        return ((ViewState["MinDate"] == null) ? new PersianDateTime(1, 1, 1) : (PersianDateTime)ViewState["MinDate"]);
        //    }
        //    set
        //    {
        //        ViewState["MinDate"] = value;
        //    }
        //}
        //[Category("Behaviour")]
        //[DefaultValue(null)]
        //[Bindable(true)]
        //public PersianDateTime MaxDate
        //{
        //    get
        //    {
        //        return ((ViewState["MaxDate"] == null) ? new PersianDateTime(3000, 1, 1) : (PersianDateTime)ViewState["MaxDate"]);
        //    }
        //    set
        //    {
        //        ViewState["MaxDate"] = value;
        //    }
        //}

        private string PrevText
        {
            get { return (string)(ViewState["prevText"] ?? ""); }
            set { ViewState["prevText"] = value; }
        }

        #endregion Properties

        protected override void OnLoad(EventArgs e)
        {
            bool found = false;
            foreach (string k in ViewState.Keys)
            {
                if (k == "Date")
                    found = true;
            }
            //if (!found)
            //    ViewState["Date"] = PersianDateTime.Now;
            base.OnLoad(e);
        }
        private void ReadText()
        {
            EnsureChildControls();
            if (ReadOnly)
                return;
            if (textBox.Text == PrevText && Mode == DatePickerMode.Date)
            {
                if (textBox.Text != "")
                    ViewState["Date"] = PersianDateTime.Parse(textBox.Text);
                return;
            }
            if (textBox.Text.Trim() == "")
            {
                ViewState["Date"] = null;
                OnValueChanged(EventArgs.Empty);
                return;
            }
            //if (textRead) //|| string.IsNullOrEmpty(textBox.Text.Trim()))
            //    return;
            PersianDateTime p;
            if (Mode != DatePickerMode.Time)
            {
                if (textBox == null || textBox.Text.Trim() == string.Empty)
                    p = new PersianDateTime();
                else
                    p = PersianDateTime.Parse(textBox.Text);
            }
            else
                p = new PersianDateTime();
            if (timePicker != null)
            {
                p.Hour = timePicker.Date.Hour;
                p.Minute = timePicker.Date.Minute;
                p.Second = timePicker.Date.Second;
            }
            ViewState["Date"] = p;
            PrevText = textBox.Text;
            OnValueChanged(EventArgs.Empty);
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }
        protected override void CreateChildControls()
        {
            textBox = new TextBox();
            textBox.ID = "txt";
            //textBox.TextChanged += new EventHandler(textBox_TextChanged);
            Controls.Add(textBox);
            timePicker = new MKB.TimePicker.TimeSelector();
            timePicker.ID = "time";
            timePicker.AllowSecondEditing = true;
            Controls.Add(timePicker);
            //requiredValidator = new RequiredFieldValidator();
            //requiredValidator.ID = 

            base.CreateChildControls();
            ChildControlsCreated = true;
        }


        private void SetText()
        {
            EnsureChildControls();
            PersianDateTime p;
            if (ViewState["Date"] == null )// || (string)ViewState["Date"] == "")
            {
                p = PersianDateTime.Now;
                textBox.Text = "";
            }
            else
            {
                p = (PersianDateTime)ViewState["Date"];
                textBox.Text = p.ToString(ReverseDate ? "dd/mm/yyyy" : "yyyy/mm/dd");
            }

            timePicker.Date = (DateTime)p;
            PrevText = textBox.Text;
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Page.Header == null)
                throw new Exception("'Page.Header' property is null. Add 'runat=\"server\"' attribute to the 'HEAD' tag of the page.");
            HtmlGenericControl li = new HtmlGenericControl("link");
            li.Attributes["rel"] = "stylesheet";
            li.Attributes["type"] = "text/css";
            li.Attributes["href"] = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.PersianDatePicker.css");
            Page.Header.Controls.Add(li);

            ReadText();

            //if (!ReadOnly)
            {
                Page.ClientScript.RegisterClientScriptResource(GetType(), "Unicorn.Web.Resources.js.PersianDatePicker.js");
                if (ReverseDate)
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "dateformat", "defaultDateFormat='dmy';", true);
            }
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            SetText();
            if (Mode == DatePickerMode.Date || Mode == DatePickerMode.DateTime)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                if (!ReadOnly)
                {
                    textBox.Height = Height;
                    textBox.Width = new Unit(97, UnitType.Percentage);
                    textBox.Attributes["onblur"] = "dp_TextLostFocus('" + textBox.ClientID + "');";
                    //textBox.ReadOnly = ReadOnly;
                    textBox.RenderControl(writer);
                }
                else
                {
                    writer.Write(textBox.Text);
                    Controls.Remove(textBox);
                }
                writer.RenderEndTag();
                if (!ReadOnly)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.AddAttribute(HtmlTextWriterAttribute.Alt, "...");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.calendar.gif"));
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "displayDatePicker(document.getElementById('" + textBox.ClientID + "'));");
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();//img
                    writer.RenderEndTag();//TD
                }
            }
            if (Mode == DatePickerMode.Time || Mode == DatePickerMode.DateTime)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                timePicker.ReadOnly = ReadOnly;
                timePicker.RenderControl(writer);
                writer.RenderEndTag();
            }
            else
                Controls.Remove(timePicker);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue(DatePickerBindingProperty.Text)]
        public DatePickerBindingProperty BindingProperty { get; set; }

        public object Value
        {
            get
            {
                switch (BindingProperty)
                {
                    case DatePickerBindingProperty.Date:
                        return Date;
                    case DatePickerBindingProperty.PersianDate:
                        return PersianDate;
                }
                return Text;
            }
            set
            {
                if (value == null)
                {
                    Text = null;
                    Date = null;
                }
                else if (value is string)
                    Text = (string)value;
                else
                    Date = (DateTime)value;
            }
        }
    }

    public enum DatePickerMode
    {
        Date,
        Time,
        DateTime,
    }
    public enum DatePickerBindingProperty
    {
        Text,
        Date,
        PersianDate
    }

}

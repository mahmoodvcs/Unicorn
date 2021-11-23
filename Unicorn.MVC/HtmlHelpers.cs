using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Unicorn.Mvc.Controllers;

namespace Unicorn.Mvc
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString DropDownForEnum<TModel, TProperty>(this HtmlHelper<TModel> html
         , Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var memberInfo = expression.MemberInfo();
            var type = ((PropertyInfo)memberInfo).PropertyType;
            if (!type.IsEnum)
                throw new Exception("The member must be an Enum");
            var builder = new TagBuilder("select");
            builder.GenerateId(memberInfo.Name);
            builder.MergeAttribute("name", expression.MemberName());
            object value = GetValue(html, expression);

            foreach (var name in Enum.GetNames(type))
            {
                var title = TitleAttribute.GetTitle(type.GetField(name, BindingFlags.Public | BindingFlags.Static), name);
                TagBuilder op = new TagBuilder("option");
                var thisVal = (int)Enum.Parse(type, name);
                op.Attributes["value"] = thisVal.ToString();
                if (value != null && (int)value == thisVal)
                    op.Attributes["selected"] = "selected";
                op.SetInnerText(title);
                builder.InnerHtml += op.ToString();
            }
            if (htmlAttributes != null)
                foreach (var prop in htmlAttributes.GetType().GetProperties(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    builder.Attributes[prop.Name] = prop.GetValue(htmlAttributes, null).ToString();
                }
            return MvcHtmlString.Create(builder.ToString());
        }
        public static MvcHtmlString DropDownForEnum<EnumType>(this HtmlHelper html, string name, EnumType? value, object htmlAttributes = null, bool addEmptyItem = false, string emptyItemValue = null) where EnumType:struct
        {
            var type = typeof(EnumType);
            if (!type.IsEnum)
                throw new Exception("The member must be an Enum");
            var builder = new TagBuilder("select");
            builder.GenerateId(name);
            builder.MergeAttribute("name", name);
            if (addEmptyItem)
            {
                TagBuilder op = new TagBuilder("option");
                op.Attributes["value"] =  emptyItemValue;
                builder.InnerHtml += op.ToString();
            }
            foreach (var n in Enum.GetNames(type))
            {
                var title = TitleAttribute.GetTitle(type.GetField(n, BindingFlags.Public | BindingFlags.Static), n);
                TagBuilder op = new TagBuilder("option");
                var thisVal = (int)Enum.Parse(type, n);
                op.Attributes["value"] = thisVal.ToString();
                //if (value != null && (int)value.Value == thisVal)
                  //  op.Attributes["selected"] = "selected";
                op.SetInnerText(title);
                builder.InnerHtml += op.ToString();
            }
            if (htmlAttributes != null)
                foreach (var prop in htmlAttributes.GetType().GetProperties(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    builder.Attributes[prop.Name] = prop.GetValue(htmlAttributes, null).ToString();
                }
            return MvcHtmlString.Create(builder.ToString());
        }
        private static object GetValue<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var meta = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var value = meta.Model;
            return value;
        }
        public static string MemberName<T, V>(this Expression<Func<T, V>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new NullReferenceException("Expression must be a member expression");
            return memberExpression.Member.Name;
        }
        private static MemberInfo MemberInfo<T, V>(this Expression<Func<T, V>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new NullReferenceException("Expression must be a member expression");
            return memberExpression.Member;
        }


        public static MvcHtmlString UIMessages(this HtmlHelper html)
        {
            if (html.ViewContext.TempData["Unicorn.Message"] == null)
                return null;
            UIMessage msg = html.ViewContext.TempData["Unicorn.Message"] as UIMessage;
            if (msg == null)
                return null;
            TagBuilder div = new TagBuilder("div");
            div.AddCssClass("alert");
            div.AddCssClass("alert-" + msg.Type.ToString().ToLower());
            if (msg.Closable)
            {
                div.AddCssClass("alert-dismissible");
                div.InnerHtml =@"<button type='button' class='close' data-dismiss='alert'>
            <span aria-hidden='true'>&times;</span><span class='sr-only'>Close</span>
        </button>";
            }
            div.Attributes["role"] = "alert";
            div.InnerHtml += msg.Message;
            return new MvcHtmlString(div.ToString());
        }
    
    }
}

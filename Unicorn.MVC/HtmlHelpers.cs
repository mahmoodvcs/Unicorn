using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Unicorn.MVC
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
                    builder.Attributes[prop.Name] = prop.GetValue(htmlAttributes).ToString();
                }
            return MvcHtmlString.Create(builder.ToString());
        }
        private static object GetValue<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var meta = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var value = meta.Model;
            return value;
        }
        private static string MemberName<T, V>(this Expression<Func<T, V>> expression)
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

    }
}

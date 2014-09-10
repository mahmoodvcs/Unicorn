using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unicorn.Web.Security.Authorization
{
	/// <summary>
	/// Title for the AuthorizedAction that will be displayed to user (in ManageAccesses control)
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class TitleAttribute : Attribute
	{
		public TitleAttribute(string title)
		{
			this.title = title;
		}
		private string title;

		public string Title
		{
			get { return title; }
		}
		
		public static string GetTitle<EnumType>()
		{
			var t=typeof(EnumType);
			return GetTitle(t, t.Name);
		}
		public static string GetTitle<EnumType>(EnumType member)
		{
            return GetTitle<EnumType>(member.ToString());
        }
        public static string GetTitle<EnumType>(string memberName)
        {
            var t = typeof(EnumType);
            return GetTitle(t.GetField(memberName, BindingFlags.Public | BindingFlags.Static), memberName, t);
        }
		public static string GetTitle(ICustomAttributeProvider typeMember, string name = null, Type containingType = null)
		{
			object[] attrs = typeMember.GetCustomAttributes(typeof(TitleAttribute), false);
			string title = null;
			if (attrs.Length > 0)
				title = ((TitleAttribute)attrs[0]).Title;
			else
				title = name;
			return title;
		}

	}
}

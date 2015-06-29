using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unicorn
{
	/// <summary>
	/// Title for the AuthorizedAction that will be displayed to user (in ManageAccesses control)
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class TitleAttribute : Attribute
	{
		public TitleAttribute(string title)
		{
            this.Title = title;
		}

		public string Title
		{
            get;
            private set;
		}

        public static string GetTitle<EnumType>() where EnumType : struct, IConvertible
		{
			var t=typeof(EnumType);
			return GetTitle(t, t.Name);
		}
        public static string GetTitle<EnumType>(EnumType member) where EnumType : struct, IConvertible
        {
            return GetTitle<EnumType>(member.ToString());
        }
        public static string GetTitle(Type enumType, string memberName)
        {
            return GetTitle(enumType.GetField(memberName, BindingFlags.Public | BindingFlags.Static), memberName, enumType);
        }
        public static string GetTitle(Type enumType, object memberValue)
        {
            var name = Enum.GetName(enumType, memberValue);
            return GetTitle( enumType.GetField(name, BindingFlags.Public | BindingFlags.Static), name, enumType);
        }
        public static string GetTitle<EnumType>(string memberName) where EnumType : struct, IConvertible
        {
            var t = typeof(EnumType);
            return GetTitle(t, memberName);
        }
        public static string GetTitle(Type type)
        {
            return GetTitle((ICustomAttributeProvider)type, type.Name, null);
        }
        public static string GetTitle(PropertyInfo typeMember)
        {
            return GetTitle((ICustomAttributeProvider)typeMember, null, null);
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

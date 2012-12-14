using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Unicorn.Web.Security.Configuration
{
	public class AccessActionsCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new AccessActionsSection();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((AccessActionsSection)element).Name;
		}
		//public override ConfigurationElementCollectionType CollectionType
		//{
		//	get
		//	{
		//		return  ConfigurationElementCollectionType.
		//	}
		//}
	}
	public class AccessActionsSection : ConfigurationSection
	{
		[ConfigurationProperty( "name" )]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}
		[ConfigurationProperty( "title" )]
		public string Title
		{
			get { return (string)this["title"]; }
			set { this["title"] = value; }
		}

		[ConfigurationProperty("", IsDefaultCollection = true)]
		public AccessActionsCollection AccessActions
		{
			get { return (AccessActionsCollection)base["AccessActions"]; }
		}

	}
}

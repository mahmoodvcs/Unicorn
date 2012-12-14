using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Unicorn.Web.Security.Configuration
{
	public class MasterPageSection : ConfigurationSection
	{
		[ConfigurationProperty( "path" )]
		public string Path
		{
			get { return (string)this["path"]; }
			set { this["path"] = value; }
		}
		[ConfigurationProperty( "contentPlaceHolderID" )]
		public string ContentPlaceHolderID
		{
			get { return (string)this["contentPlaceHolderID"]; }
			set { this["contentPlaceHolderID"] = value; }
		}
	}
}

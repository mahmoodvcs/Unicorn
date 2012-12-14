using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Unicorn.Web.Security.Configuration
{
	public class UniSecuritySectionGroup : ConfigurationSectionGroup
	{
		[ConfigurationProperty("masterPage")]
		public MasterPageSection MasterPage
		{
			get { return (MasterPageSection)base.Sections["masterPage"]; }
		}
		[ConfigurationProperty("accessActions")]
		public AccessActionsSection AccessActions
		{
			get { return (AccessActionsSection)base.Sections["accessActions"]; }
		}
	}
}

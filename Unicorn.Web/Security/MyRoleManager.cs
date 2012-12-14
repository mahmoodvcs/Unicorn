using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Configuration;

namespace Unicorn.Web.Security
{
	internal class MyRoleManager
	{
		public const string ProviderName = "JcoRoleProvider";

		public static void SetRoleSettings(System.Web.Configuration.RoleManagerSection roleManagerSection)
		{
			SetRoleSettings(roleManagerSection, Unicorn.Data.ConnectionManager.ConnectionStringName);
		}
		public static void SetRoleSettings(System.Web.Configuration.RoleManagerSection roleManagerSection
			, string connectionStringName)
		{
			roleManagerSection.Enabled = true;
			roleManagerSection.DefaultProvider = ProviderName;
			if (roleManagerSection.Providers[ProviderName] == null)
			{
				ProviderSettings p = new ProviderSettings( ProviderName, "System.Web.Security.SqlRoleProvider" );
				p.Parameters["connectionStringName"] = connectionStringName;
				p.Parameters["applicationName"] = "/";
				roleManagerSection.Providers.Add(p);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using System.Configuration;

namespace Unicorn.Web.Security
{
	internal class UniMembershipProvider : SqlMembershipProvider
	{
        public const string ProviderName = "UniMembershipProvider";

		internal static void SetPrividerSettings(MembershipSection membershipSection)
		{
			SetPrividerSettings(membershipSection, Unicorn.Data.ConnectionManager.ConnectionStringName);
		}
		internal static void SetPrividerSettings( MembershipSection membershipSection, string connectionStringName )
		{
			membershipSection.DefaultProvider = ProviderName;
			ProviderSettings providerSettings = membershipSection.Providers[ProviderName];
			if ( providerSettings == null )
			{
				providerSettings = new ProviderSettings( ProviderName, "Unicorn.Web.Security.UniMembershipProvider" );//this.GetType().Name
				providerSettings.Parameters["enablePasswordRetrieval"] = "false";
				providerSettings.Parameters["connectionStringName"] = connectionStringName;
				providerSettings.Parameters["enablePasswordReset"] = "true";
				providerSettings.Parameters["requiresQuestionAndAnswer"] = "true";
				providerSettings.Parameters["requiresUniqueEmail"] = "true";
				providerSettings.Parameters["passwordFormat"] = "Clear";
                providerSettings.Parameters["minRequiredNonalphanumericCharacters"] = "0";
				providerSettings.Parameters["minRequiredPasswordLength"] = "4";
				providerSettings.Parameters["applicationName"] = "/";
				membershipSection.Providers.Add(providerSettings);
			}
		}

	}
}

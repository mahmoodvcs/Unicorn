using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using System.Configuration;

namespace Jco.Web.Security
{
	internal class JcoMembershipProvider : SqlMembershipProvider
	{
		public const string ProviderName = "JcoMembershipProvider";

		internal static void SetPrividerSettings(MembershipSection membershipSection)
		{
			SetPrividerSettings(membershipSection, Jco.Data.GeneralDataHandler.ConnecionStringName);
		}
		internal static void SetPrividerSettings( MembershipSection membershipSection, string connectionStringName )
		{
			membershipSection.DefaultProvider = ProviderName;
			ProviderSettings providerSettings = membershipSection.Providers[ProviderName];
			if ( providerSettings == null )
			{
				providerSettings = new ProviderSettings( ProviderName, "Jco.Web.Security.JcoMembershipProvider" );//this.GetType().Name
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

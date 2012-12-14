using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

namespace Unicorn.Web.Security.Configuration
{
	public static class ConfigUtility
	{
        private static System.Configuration.Configuration websiteConfig = null;

		public static System.Configuration.Configuration GetConfigFile()
		{
            if (websiteConfig == null)
            {
                WebConfigurationFileMap fileMap = new WebConfigurationFileMap();
                fileMap.VirtualDirectories.Add("/", new VirtualDirectoryMapping(HttpContext.Current.Server.MapPath("~"), true));
                try
                {
                    websiteConfig = WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/");
                }
                catch (ArgumentOutOfRangeException)
                {
                    websiteConfig = WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/", "t");
                }
            }
            return websiteConfig;
		}
		public static SystemWebSectionGroup GetSystemWebSectionGroup( System.Configuration.Configuration conf )
		{
			//return WebConfigurationManager.GetWebApplicationSection("system.web") as SystemWebSectionGroup;
			return conf.GetSectionGroup( "system.web" ) as SystemWebSectionGroup;
		}
		public static SystemWebSectionGroup GetSystemWebSectionGroup()
		{
			System.Configuration.Configuration conf = GetConfigFile();
			return GetSystemWebSectionGroup( conf );
		}
		public static UniSecuritySectionGroup GetJcoSecuritySection()
		{
			//return (JcoSecuritySectionGroup)WebConfigurationManager.GetWebApplicationSection("uniSecurity");
			System.Configuration.Configuration conf = GetConfigFile();
			return (UniSecuritySectionGroup)conf.GetSectionGroup( "uniSecurity" );
		}

	}

}

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Xml;

namespace Unicorn.Web.Security.Configuration
{
	public class WebConfigInitializer
	{
		public static void InitWebConfig(System.Configuration.Configuration conf)
		{
			InitWebConfig(conf, false, Unicorn.Data.ConnectionManager.ConnectionStringName);
		}
		public static void InitWebConfig( System.Configuration.Configuration conf, bool useAcessProviders, string connectionStringName )
		{
			SystemWebSectionGroup websec = conf.GetSectionGroup( "system.web" ) as SystemWebSectionGroup;
			websec.Authentication.Mode = AuthenticationMode.Forms;
			//if (useAcessProviders)
			//{
			//	AccessMembershipProvider.SetPrividerSettings(websec.Membership, connectionStringName);
			//	AccessRoleProvider.SetRoleSettings(websec.RoleManager, connectionStringName);
			//}
			//else
			//{
				UniMembershipProvider.SetPrividerSettings(websec.Membership, connectionStringName);
				MyRoleManager.SetRoleSettings(websec.RoleManager, connectionStringName);
			//}
            CreateHttpHandler(websec);
            InitConfigSections(ref conf);
			InitJcoSecuritySection( conf );
			conf.Save();
		}

        public static void CreateHttpHandler(SystemWebSectionGroup websec)
        {
            HttpHandlersSection httpHandlers = websec.HttpHandlers;
            bool found = false;
            foreach (HttpHandlerAction handler in httpHandlers.Handlers)
            {
                if (handler.Path == UniHttpHandler.HandlerPath)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                HttpHandlerAction handler = new HttpHandlerAction(UniHttpHandler.HandlerPath, "Unicorn.Web.Security.JcoHttpHandler, Unicorn.Web", "*");
                httpHandlers.Handlers.Add(handler);
            }
        }
        private static void InitConfigSections(ref System.Configuration.Configuration conf)
		{
			if ( conf.GetSectionGroup( "uniSecurity" ) != null )
				return;
			XmlDocument doc = new XmlDocument();
			doc.Load( conf.FilePath );
			XmlNode confNode = doc["configuration"];
			XmlNode confSections = confNode["configSections"];
			if ( confSections == null )
			{
				confSections = doc.CreateElement( "configSections" );
				confNode.InsertBefore( confSections, confNode.FirstChild );
			}
			if ( !HasJcoSecurityInConfigSection( confSections ) )
			{
				XmlElement jcoSec = doc.CreateElement( "sectionGroup" );
				XmlAttribute attr = doc.CreateAttribute( "name" );
				attr.Value = "uniSecurity";
				jcoSec.Attributes.Append( attr );
				attr = doc.CreateAttribute( "type" );
				attr.Value = "Unicorn.Web.Security.Configuration.UniSecuritySectionGroup, Unicorn.Web";
				jcoSec.Attributes.Append( attr );

				XmlElement masterPage = doc.CreateElement( "section" );
				attr = doc.CreateAttribute( "name" );
				attr.Value = "masterPage";
				masterPage.Attributes.Append( attr );
				attr = doc.CreateAttribute( "type" );
				attr.Value = "Unicorn.Web.Security.Configuration.MasterPageSection, Unicorn.Web";
				masterPage.Attributes.Append( attr );
				
				jcoSec.AppendChild( masterPage );
				confSections.AppendChild( jcoSec );

				CreateJcoSecuritySection( confNode );
				doc.Save( conf.FilePath );
				conf = ConfigUtility.GetConfigFile();
			}
		}

		private static bool HasJcoSecurityInConfigSection( XmlNode confSections )
		{
			foreach ( XmlNode ch in confSections.ChildNodes )
			{
				if ( ch.NodeType != XmlNodeType.Element )
					continue;
				if ( ch.Attributes["name"].Value == "uniSecurity" )
					return true;
			}
			return false;
		}

		private static void CreateJcoSecuritySection( XmlNode confNode )
		{
			XmlDocument doc = confNode.OwnerDocument;
			XmlElement jcoSec = doc.CreateElement( "uniSecurity" );
			confNode.AppendChild( jcoSec );

			XmlElement masterPage = doc.CreateElement( "masterPage" );
			XmlAttribute attr = doc.CreateAttribute( "path" );
			attr.Value = "";
			masterPage.Attributes.Append( attr );
			attr = doc.CreateAttribute( "contentPlaceHolderID" );
			attr.Value = "";
			masterPage.Attributes.Append( attr );

			jcoSec.AppendChild( masterPage );
		}

        private static void InitJcoSecuritySection( System.Configuration.Configuration conf )
		{
			UniSecuritySectionGroup jcoSec = (UniSecuritySectionGroup)conf.GetSectionGroup( "uniSecurity" );
			if ( jcoSec == null )
				throw new Exception("اشكالي در انجام تنضيمات امنيتي در سايت بوجود آمده است. لطفا تنظيمات را به صورت دستي انجام دهيد. \r\n'uniSecurity' is null. Correct the web.config file.");
			if ( jcoSec.MasterPage.Path == "" )
			{
				string contentPlaceholderID;
				jcoSec.MasterPage.Path = GetMasterPagePath( out contentPlaceholderID );
				jcoSec.MasterPage.ContentPlaceHolderID = contentPlaceholderID;
			}
		}
		private static string GetMasterPagePath( out string contentPlaceholderID )
		{
			contentPlaceholderID = "ContentPlaceHolder1";
			return "~/MasterPage1.master";
		}
	}
}

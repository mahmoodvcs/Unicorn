using System;
using System.Collections.Generic;
using System.Text;
using Unicorn.Web.Security.Configuration;
using System.Web.UI;

namespace Unicorn.Web.Security.Pages
{
	public class PageWithMaster : PageBase
	{
		public PageWithMaster()
			: this(true)
		{
		}

		public PageWithMaster(bool useMasterPage)
			: base(useMasterPage ? IsMasterPageSet : false)
		{
			if (hasMasterPage)
			{
				jcoSec = ConfigUtility.GetJcoSecuritySection();
				AppRelativeVirtualPath = "~/" + UniHttpHandler.HandlerPath;
                if (jcoSec.MasterPage.Path == "")
                    hasMasterPage = false;
                else
				    MasterPageFile = jcoSec.MasterPage.Path;
			}
		}
        private static bool IsMasterPageSet
        {
            get
            {
                return ConfigUtility.GetJcoSecuritySection().MasterPage.Path != "";
            }
        }
		UniSecuritySectionGroup jcoSec;

		protected Control GetContentPlaceHolder()
		{
			if (hasMasterPage)
				return Master.FindControl(jcoSec.MasterPage.ContentPlaceHolderID);
			else
				return null;
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: Mahmood
 * Date: 2014/01/14
 * Time: 3:43 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Configuration.Provider;
using System.Web.Security;

namespace Unicorn.Web.Security
{
	/// <summary>
	/// Description of AutoUnlockMembershipProvider.
	/// </summary>
	public class AutoUnlockMembershipProvider: SqlMembershipProvider
	{
		public AutoUnlockMembershipProvider()
		{
		}
		
		private int autoUnlockTimeout = 60;    //Default to 60 minutes
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			string sunlockTimeOut = config["autoUnlockTimeout"];
			if (!String.IsNullOrEmpty(sunlockTimeOut))
			 autoUnlockTimeout = Int32.Parse(sunlockTimeOut);
			config.Remove("autoUnlockTimeout");
			base.Initialize(name, config);
		}
		
		private bool AutoUnlockUser(string username)
		{
			MembershipUser mu = this.GetUser(username,false);
			if ((mu != null) &&
			  (mu.IsLockedOut) &&
			  (mu.LastLockoutDate.ToUniversalTime().AddMinutes(autoUnlockTimeout) < DateTime.UtcNow))
			{
			  bool retval = mu.UnlockUser();
			  if (retval)
			     return true;
			  else
			     return false;    //something went wrong with the unlock
			}
			else
			  return false;       //not locked out in the first place
		                      //or still in lockout period
		}
		
		public override bool ValidateUser(string username, string password)
		{
		   bool retval = base.ValidateUser(username, password);
		   //The account may be locked out at this point
		   if (retval == false)
		   {
		      bool successfulUnlock = AutoUnlockUser(username);
		      if (successfulUnlock)
		         //re-attempt the login
		            return base.ValidateUser(username, password);
		      else
		         return false;
		   }
		   else
		      return retval;    //first login was successful
		}
		
		public override string ResetPassword(string username, string passwordAnswer)
		{
		   //A MembershipPasswordException could be due to a lockout
		   try
		   {
		      return base.ResetPassword(username, passwordAnswer);
		   }
		   catch (MembershipPasswordException me) {}
		   bool successfulUnlock = AutoUnlockUser(username);
		   if (successfulUnlock)
		      //re-attempt the password reset
		      return base.ResetPassword(username, passwordAnswer);
		   else
		      throw new ProviderException("The attempt to auto unlock the user failed during ResetPassword.");
		}
	}
}

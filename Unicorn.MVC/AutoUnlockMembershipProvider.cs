﻿using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Unicorn.Mvc
{
    public class AutoUnlockMembershipProvider : System.Web.Providers.DefaultMembershipProvider
    {
        public int AutoUnlockTimeout { get; private set; } = 60;
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            string sunlockTimeOut = config["autoUnlockTimeout"];
            if (!String.IsNullOrEmpty(sunlockTimeOut))
                AutoUnlockTimeout = Int32.Parse(sunlockTimeOut);
            config.Remove("autoUnlockTimeout");
            base.Initialize(name, config);
        }

        private bool AutoUnlockUser(string username)
        {
            MembershipUser mu = this.GetUser(username, false);
            if ((mu != null) &&
              (mu.IsLockedOut) &&
              (mu.LastLockoutDate.ToUniversalTime().AddMinutes(AutoUnlockTimeout) < DateTime.UtcNow))
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
            catch (MembershipPasswordException) { }
            bool successfulUnlock = AutoUnlockUser(username);
            if (successfulUnlock)
                //re-attempt the password reset
                return base.ResetPassword(username, passwordAnswer);
            else
                throw new ProviderException("The attempt to auto unlock the user failed during ResetPassword.");
        }
    }
}
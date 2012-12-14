using System;
using System.Collections.Generic;
using System.Text;

namespace Unicorn.Web.Security.Authorization
{
    /// <summary>
    /// Title for the AuthorizedAction that will be displayed to user (in ManageAccesses control)
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class TitleAttribute : Attribute
    {
        public TitleAttribute(string title)
        {
            this.title = title;
        }
        private string title;

        public string Title
        {
            get { return title; }
        }
    }
}

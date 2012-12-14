using System;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Unicorn.Web;

namespace Unicorn.Web.Security.Authorization
{
	public class AuthorizedAction
	{
		private AuthorizedAction()
		{
            subActions = new ObservableCollection<AuthorizedAction>();
            subActions.ItemInserted += subActions_ItemInserted;
		}

        void subActions_ItemInserted(object sender, ListItemEventArgs<AuthorizedAction> e)
        {
            e.Item.ParentAction = this;
        }
		public AuthorizedAction(string name)
			: this()
		{
            int i = name.IndexOf('.');
            if (i > 0)
            {
                this.name = name.Substring(0, i);
                AddSubAction(name.Substring(i));
            }
            else
                this.name = name;
            //string[] ss = name.Split('.');
            //this.name = ss[0];
            //AuthorizedAction ac = this;
            //for (int i = 1; i < ss.Length;i++ )
            //{
            //    ac = ac.AddSubAction(ss[i]);
            //}
		}
		public AuthorizedAction(string name, string subAction)
			: this(name)
		{
			SubActions.Add(new AuthorizedAction(subAction));
		}
		public AuthorizedAction(string name, params string[] subActions)
			: this(name)
		{
			//AuthorizedAction aa = this;
			//foreach (string action in subActionsHierarchy)
			//{
			//	AuthorizedAction a = new AuthorizedAction(action);
			//	aa.Add(a);
			//	aa = a;
			//}

            foreach (string s in subActions)
			{
				AddSubAction(s);
			}
		}
		//public AuthorizedAction(string name, params AuthorizedAction[] subActions)
		//	: this(name)
		//{
		//	this.subActions = new List<AuthorizedAction>(subActions);
		//}

		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string FullName
        {
            get
            {
                if (ParentAction == null)
                    return name;
                return ParentAction.FullName + "." + name;
            }
        }

        public AuthorizedAction ParentAction { get; internal set; }

        ObservableCollection<AuthorizedAction> subActions;
        public ObservableCollection<AuthorizedAction> SubActions { get { return subActions; } }

        public AuthorizedAction AddSubAction(string name)
        {
            string[] ss = Utility.SplitString(name, '.');
            AuthorizedAction ac = this;
            foreach (string s in ss)
            {
                ac = ac.AddSubActionInternal(s);
            }
            return ac;
        }

        private AuthorizedAction AddSubActionInternal(string name)
        {
            AuthorizedAction ac = this[name];
            if (ac == null)
                SubActions.Add(ac = new AuthorizedAction(name));
            return ac;
        }

        /// <summary>
        /// Returns a sub action of this action with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public AuthorizedAction this[string name]
		{
			get
			{
				foreach (AuthorizedAction a in SubActions)
				{
					if (a.name == name)
						return a;
				}
				return null;
			}
		}
		public bool ContainsAction(AuthorizedAction action)
		{
			AuthorizedAction ac = this[action.name];
            if (ac == null)
				return false;
			if (action.SubActions.Count == 0)
				return true;
			foreach (AuthorizedAction sub in action.SubActions)
			{
				if (ac.ContainsAction(sub))
					return true;
			}
			return false;
		}
	}
}

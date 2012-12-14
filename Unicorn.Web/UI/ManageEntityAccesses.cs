using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using Unicorn.Data;

namespace Unicorn.Web.UI
{
    [ToolboxData("<{0}:JcoManageAccesses runat=server></{0}:JcoManageAccesses>")]
    public class ManageEntityAccesses : CompositeControl
    {
        public ManageEntityAccesses()
        {
        }
        ManageAccesses uxAccesses;
        static string[] aspnetTables = new string[]
        {
            "aspnet_Applications", "aspnet_Membership", "aspnet_Profile", "aspnet_Roles", "aspnet_SchemaVersions",
            "aspnet_UserRoleActions", "aspnet_Users", "aspnet_UsersInRoles"
        };

        protected override void CreateChildControls()
        {
            uxAccesses = new ManageAccesses();
            //uxAccesses.ActionPrefix = "Entity";
            Controls.Add(uxAccesses);
            ChildControlsCreated = true;
            base.CreateChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();
            if (!Page.IsPostBack)
            {
                DataTable dt = SqlHelper.ExecuteProcedure("sp_tables",
                    new SqlParameter("@table_type", "\"'TABLE'\""));
                TreeNode node = new TreeNode("جداول","Entity");
                uxAccesses.ActionsTree.Nodes.Add(node);
                foreach (DataRow row in dt.Rows)
                {
                    string tn = row["TABLE_NAME"].ToString();
                    if (Array.IndexOf(aspnetTables, tn) >= 0)
                        continue;
                    TreeNode n = new TreeNode(SchemaUtility.GetSqlTableDescription(tn, row["TABLE_OWNER"].ToString()), tn);
                    n.ChildNodes.Add(new TreeNode("نمايش", "View"));
                    n.ChildNodes.Add(new TreeNode("ويرايش", "Edit"));
                    n.ChildNodes.Add(new TreeNode("حذف", "Delete"));
                    node.ChildNodes.Add(n);
                }
            }
            base.OnLoad(e);
        }
    }
}

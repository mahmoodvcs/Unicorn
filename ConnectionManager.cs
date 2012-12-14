using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Threading;
using System.Configuration;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data.EntityClient;

namespace Unicorn.Data
{
    public static class ConnectionManager
    {
        static ConnectionManager()
        {
            if (WebConfigurationManager.ConnectionStrings.Count == 0)
                throw new Exception("No ConnectionString is found in Web.Config file.");
            else if (WebConfigurationManager.ConnectionStrings.Count == 1)
                ConnectionStringName = WebConfigurationManager.ConnectionStrings[0].Name;
            else
                ConnectionStringName = WebConfigurationManager.ConnectionStrings[1].Name;
        }
        private static DbProviderFactory dbProviderFactory;
        public static DbProviderFactory DbProviderFactory
        {
            get { return dbProviderFactory; }
            set { dbProviderFactory = value; }
        }
        private static string providerName = "System.Data.SqlClient";

        private const string connectionStringSessionName = "_Unicorn.Data.DataHandler_ConStr_";
        static string connectionString = "";
        public static string ConnectionString
        {
            get { return connectionString; }
            set
            {
                connectionString = value;
            }
        }

        static DatabaseType databaseType;
        public static DatabaseType DatabaseType
        {
            get { return databaseType; }
        }
        static string connectionStringName = "";
        public static string ConnectionStringName
        {
            get { return connectionStringName; }
            set
            {
                connectionStringName = value;
                ConnectionStringSettings cn = ConfigurationManager.ConnectionStrings[connectionStringName];
                connectionString = cn.ConnectionString;
                providerName = cn.ProviderName;
                dbProviderFactory = DbProviderFactories.GetFactory(cn.ProviderName);
                switch (cn.ProviderName)
                {
                    case "":
                    case "System.Data.SqlClient":
                        databaseType = DatabaseType.SQLServer;
                        break;
                    case "System.Data.SQLite":
                        databaseType = DatabaseType.SQLite;
                        break;
                }
            }
        }

        private static Dictionary<int, DbConnection> _connections = new Dictionary<int, DbConnection>();

        public static DbConnection Connection
        {
            get
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                DbConnection con;
                if (!_connections.TryGetValue(threadId, out con))
                {
                    con = CreateConnection();
                    _connections[threadId] = con;
                }
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();
                return con;
            }
        }

        private static DbConnection CreateConnection()
        {
            DbConnection con = GetConnection(); //new SqlConnection(UserConnectionString);
            con.Open();
            return con;
        }
        private static DbConnection GetConnection()
        {
            DbConnection con = null;
            switch (databaseType)
            {
                case DatabaseType.SQLServer:
                    con = new SqlConnection(ConnectionString);
                    break;
                case DatabaseType.SQLite:
                    throw new NotImplementedException();
                //    con = new SQLiteConnection(UserConnectionString);
                //break;
            }
            return con;
        }
        public static EntityConnection GetEntitiyConnection()
        {
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = providerName;
            entityBuilder.ProviderConnectionString = ConnectionString;
            entityBuilder.Metadata = @"res://*";
            EntityConnection conn = new EntityConnection(entityBuilder.ToString());
            //string cn = "metadata=res://*/Workflow.WorkflowModel.csdl|res://*/Workflow.WorkflowModel.ssdl|res://*/Workflow.WorkflowModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;" + Unicorn.Data.SqlHelper.ConnecionString + "&quote;";
            return conn;
        }

    }
}


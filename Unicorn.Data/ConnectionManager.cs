using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Collections;
using System.Threading;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Configuration;
using System.Collections.Concurrent;

namespace Unicorn.Data
{
    public class ConnectionManager
    {
        #region Consrcutor

        protected ConnectionManager()
        {
        }

        static ConnectionManager()
        {
            if (WebConfigurationManager.ConnectionStrings.Count == 0)
            {
                //throw new Exception("No ConnectionString is found in Web.Config file.");
            }
            else if (WebConfigurationManager.ConnectionStrings.Count == 1)
                ConnectionStringName = WebConfigurationManager.ConnectionStrings[0].Name;
            else
                ConnectionStringName = WebConfigurationManager.ConnectionStrings[1].Name;
        }

        #endregion

        Type connectionType;

        public void Init<T>() where T : DbConnection
        {
            connectionType = typeof(T);
            connectionConstructor = connectionType.GetConstructor(new Type[] { typeof(string) });
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
            set
            {
                databaseType = value;
                switch (databaseType)
                {
                    case Data.DatabaseType.SqlEC:
                        providerName = "System.Data.SqlServerCe.4.0";
                        break;
                    case Data.DatabaseType.SQLServer:
                        providerName = "System.Data.SqlClient";
                        break;
                    case Data.DatabaseType.SQLite:
                        providerName = "System.Data.SQLite";
                        break;
                }
                dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            }
        }
        static string connectionStringName = "";
        public static string ConnectionStringName
        {
            get { return connectionStringName; }
            set
            {
                connectionStringName = value;
                ConnectionStringSettings cn = WebConfigurationManager.ConnectionStrings[connectionStringName];
                if (cn == null)
                    throw new Exception("Connection string does not exist.");
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
                    case "System.Data.SqlServerCe.4.0":
                        databaseType = Data.DatabaseType.SqlEC;
                        break;
                }
            }
        }


        #region Fields

        private int connectionCounter = 0;
        private int transactionCounter = 0;
        private static ConcurrentDictionary<Thread, ConnectionManager> ManagerInstances = new ConcurrentDictionary<Thread, ConnectionManager>();
        protected DbConnection connection = null;
        private static System.Reflection.ConstructorInfo connectionConstructor;
        //private static object mutex = new object();
        public DbTransaction Transaction
        {
            get;
            private set;
        }

        #endregion

        #region Properties

        public static ConnectionManager Instance
        {
            get
            {
                //lock (mutex)
                {
                    ConnectionManager connectionManager = null;
                    if (ManagerInstances.ContainsKey(Thread.CurrentThread))
                    {
                        connectionManager = ManagerInstances[Thread.CurrentThread];
                    }

                    if (connectionManager == null)
                    {
                        connectionManager = new ConnectionManager();
                        ManagerInstances[Thread.CurrentThread] = connectionManager;
                    }

                    return connectionManager;
                }
            }
        }

        #endregion

        #region Methods

        #region Private

        private void CloseInternalConnection()
        {
            connection.Close();
            connection = null;
            ConnectionManager temp;
            ManagerInstances.TryRemove(Thread.CurrentThread, out temp);
        }

        #endregion

        #region Protected

        protected void CreateConnection()
        {
            if (dbProviderFactory != null)
            {
                connection = dbProviderFactory.CreateConnection();
                connection.ConnectionString = connectionString;
            }
            else if (connectionConstructor != null)
            {
                connection = (DbConnection)connectionConstructor.Invoke(new object[] { ConnectionString });
            }
            else
            {
                switch (databaseType)
                {
                    case DatabaseType.SQLServer:
                        connection = new SqlConnection(ConnectionString);
                        break;
                    case DatabaseType.SQLite:
                        throw new NotImplementedException();
                    //    con = new SQLiteConnection(UserConnectionString);
                    //break;
                }
            }
            connection.Open();
        }

        #endregion

        #region Public

        /// <summary>
        /// Creates or returns an already created connection. All calls to this method
        /// must be paired with a call to ReleaseConncetion().
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            connectionCounter++;

            if (connectionCounter == 1 && connection == null)	//only create and open a real connection if it hasn't been opened yet
            {
                CreateConnection();
            }

            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            return connection;
        }

        public void ReleaseConnection()
        {
            connectionCounter--;

            if (connectionCounter == 0)
            {
                CloseInternalConnection();
            }
            else if (connectionCounter < 0) //this should never happen
            {
                connectionCounter = 0;
                throw new ApplicationException("Too many FreeConnections were called. The connection count is negative");
            }
        }

        public void BeginTransaction()
        {
            if (transactionCounter == 0)
            {
                Transaction = GetConnection().BeginTransaction();
            }

            transactionCounter++;
        }

        public void RollbackTransaction()
        {
            if (OpenTransactionCount > 0)
            {
                Transaction.Rollback();
                Transaction.Dispose();
                Transaction = null;
                ReleaseConnection();
                transactionCounter = 0;
            }
        }

        public void CommitTransaction()
        {
            if (OpenTransactionCount > 0)
            {
                transactionCounter--;

                if (transactionCounter == 0)
                {
                    Transaction.Commit();
                    Transaction.Dispose();
                    Transaction = null;
                    ReleaseConnection();
                }
            }
            else
            {
                throw new Exception("There is no open transaction to commit");
            }
        }

        public int OpenTransactionCount
        {
            get
            {
                return transactionCounter;
            }
        }

        #endregion

        #endregion

    }

}

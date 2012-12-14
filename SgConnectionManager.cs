using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Collections;
using System.Threading;
using System.Data.SqlClient;
using SG.Framework.Helper;
using SG.Framework.Exceptions;
using SG.Framework.Messages;

namespace SG.Framework.DAL
{
    internal class ConnectionManager
    {
        #region Consrcutor

        protected ConnectionManager()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #endregion

        #region Fields

        private int connectionCounter = 0;
        private int transactionCounter = 0;
        private static Dictionary<Thread, ConnectionManager> ManagerInstances = new Dictionary<Thread, ConnectionManager>();
        protected SqlConnection connection = null;
        private static object mutex = new object();
        public SqlTransaction Transaction
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
                lock (mutex)
                {
                    ConnectionManager connectionManager = null;
                    if (ManagerInstances.ContainsKey(Thread.CurrentThread))
                    {
                        connectionManager = ManagerInstances[Thread.CurrentThread];
                    }

                    if (connectionManager == null)
                    {
                        connectionManager = new ConnectionManager();
                        ManagerInstances.Add(Thread.CurrentThread, connectionManager);
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
            ManagerInstances.Remove(Thread.CurrentThread);
        }

        #endregion

        #region Protected

        protected void CreateConnection()
        {
            connection = new SqlConnection(DatabaseHelper.GetConnectionString());
            connection.Open();
        }

        #endregion

        #region Public

        /// <summary>
        /// Creates or returns an already created connection. All calls to this method
        /// must be paired with a call to ReleaseConncetion().
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnection()
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
                throw new SgException(new SgMessage("ThereIsNoOpenTransaction", "There is no open transaction to commit", "There is no open transaction to commit"));
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

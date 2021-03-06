﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.EntityClient;

namespace Unicorn.Data
{
    public class SqlHelper
    {
        public static int CommandTimeOut
        {
            get;
            set;
        }

        static SqlHelper()
        {
            CommandTimeOut = 600;
        }

        public static object ExecuteScaler(string command, params DbParameter[] parameters)
        {
            object res = null;
            DbCommand cmd = CreateCommand();
            cmd.CommandText = command;
            foreach (DbParameter sqp in parameters)
                cmd.Parameters.Add(sqp);
            try
            {
                res = cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw new UniSqlException(ex, command, parameters.Cast<SqlParameter>().ToArray());
            }
            ConnectionManager.Instance.ReleaseConnection();
            if (res == DBNull.Value)
                res = null;
            return res;
        }

        public static DataReader ExecuteReader(string command, params DbParameter[] parameters)
        {
            return ExecuteReader(command, CommandBehavior.Default, parameters);
        }
        public static DataReader ExecuteReader(string command, CommandBehavior behavior, params DbParameter[] parameters)
        {
            DataReader dr = null;
            DbCommand cmd = CreateCommand();
            cmd.CommandText = command;
            foreach (DbParameter p in parameters)
                cmd.Parameters.Add(p);
            try
            {
                dr = new DataReader(cmd.ExecuteReader());
            }
            catch (SqlException ex)
            {
                throw new UniSqlException(ex, command, parameters.Cast<SqlParameter>().ToArray());
            }
            return dr;
        }

        public static int ExecuteNonQuery(string command, params DbParameter[] parameters)
        {
            int n;
            //using (var tr = new TransactionScope())
            //{
            DbCommand cmd = CreateCommand();
            cmd.CommandText = command;
            foreach (DbParameter p in parameters)
                cmd.Parameters.Add(p);
            try
            {
                n = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new UniSqlException(ex, command, parameters.Cast<SqlParameter>().ToArray());
            }
            finally
            {
                ConnectionManager.Instance.ReleaseConnection();
            }
            //tr.Commit();
            //}
            return n;
        }

        public static DataTable ExecuteCommand(string command, params DbParameter[] parameters)
        {
            DataTable dt = new DataTable();
            DbCommand cmd = CreateCommand();
            cmd.CommandText = command;
            //cmd.Connection = con;
            foreach (DbParameter param in parameters)
                cmd.Parameters.Add(param);
            DbDataAdapter da = ConnectionManager.DbProviderFactory.CreateDataAdapter();
            da.SelectCommand = cmd;
            try
            {
                da.Fill(dt);
            }
            catch (SqlException ex)
            {
                throw new UniSqlException(ex, command, parameters.Cast<SqlParameter>().ToArray());
            }
            finally
            {
                ConnectionManager.Instance.ReleaseConnection();
            }
            return dt;
        }

        public static bool ExecuteNonQueryProcedure(string procName, params DbParameter[] parameters)
        {
            bool res = true;
            DbCommand cmd = CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procName;
            foreach (DbParameter param in parameters)
            {
                cmd.Parameters.Add(param);
            }
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new UniSqlException(ex, procName, parameters.Cast<SqlParameter>().ToArray());
            }
            finally
            {
                ConnectionManager.Instance.ReleaseConnection();
            }
            return res;
        }

        public static object ExecuteScalerProcedure(string procName, params DbParameter[] parameters)
        {
            //string res = "0";
            object val;
            DbCommand cmd = CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procName;
            foreach (DbParameter param in parameters)
            {
                cmd.Parameters.Add(param);
            }
            try
            {
                val = cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw new UniSqlException(ex, procName, parameters.Cast<SqlParameter>().ToArray());
            }
            finally
            {
                ConnectionManager.Instance.ReleaseConnection();
            }
            //res = val == null ? "0" : val.ToString();
            return val;
        }

        public static DataTable ExecuteProcedure(string procName, params DbParameter[] parameters)
        {
            return ExecuteProcedure(procName, -1, -1, parameters);
        }
        public static DataTable ExecuteProcedure(string procName, int startIndex, int recCount, params DbParameter[] parameters)
        {
            DbCommand cmd = CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procName;
            foreach (DbParameter param in parameters)
            {
                cmd.Parameters.Add(param);
            }
            DbDataAdapter da = ConnectionManager.DbProviderFactory.CreateDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            try
            {
                if (startIndex >= 0 && recCount > 0)
                    da.Fill(startIndex, recCount, dt);
                else
                    da.Fill(dt);
            }
            catch (SqlException ex)
            {
                throw new UniSqlException(ex, procName, parameters.Cast<SqlParameter>().ToArray());
            }
            finally
            {
                ConnectionManager.Instance.ReleaseConnection();
            }
            return dt;
        }


        private static DbCommand CreateCommand()
        {
            var con = ConnectionManager.Instance.GetConnection();
            var cmd = con.CreateCommand();
            cmd.Transaction = ConnectionManager.Instance.Transaction;
            if (ConnectionManager.DatabaseType != DatabaseType.SqlEC)
                cmd.CommandTimeout = CommandTimeOut;

            return cmd;
        }
        private static DbCommand CreateCommand(string command)
        {
            var cmd = CreateCommand();
            cmd.CommandText = command;
            return cmd;
        }
        public static DbParameter CreateParameter()
        {
            return ConnectionManager.DbProviderFactory.CreateParameter();
        }
        public static DbParameter CreateParameter(string parameterName, object value)
        {
            var p = CreateParameter();
            p.ParameterName = parameterName;
            if (value != null && value.GetType().Name.StartsWith("Sql"))
            {
                ((SqlParameter)p).SqlDbType = SqlDbType.Udt;
                ((SqlParameter)p).UdtTypeName = value.GetType().Name.Remove(0,3).ToLower();
            }
            p.Value = value;
            return p;
        }

    }
}

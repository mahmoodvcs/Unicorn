using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Linq;

namespace Unicorn.Data
{
    public static class SchemaUtility
    {
        /// <summary>
        /// Gets informations for all tables and fields for the given connection.
        /// </summary>
        /// <param name="connection">The connection to get tables from.</param>
        /// <returns>An array of TableInfo objects containing information for all tables and fields in each table.</returns>
        public static TableInfo[] GetTables(DbConnection connection)
        {
            return GetTables(connection, null, true);
        }

        /// <summary>
        /// Gets informations for all tables and fields for the given connection.
        /// </summary>
        /// <param name="connection">The connection to get tables from.</param>
        /// <param name="tableOwner">Owner of the tables to return. if 'null', all tables for
        /// the connection will be returnd.</param>
        /// <param name="readColumns">Specifies whether to get columns information for each table or not.</param>
        /// <returns>An array of TableInfo objects containing information for all tables.
        /// if <paramref name="readColumns"/> is 'true', 'Columns' property of each table contains fields for the table.</returns>
        public static TableInfo[] GetTables(DbConnection connection, string tableOwner, bool readColumns)
        {
            bool close = Helper.OpenConnection(connection);
            List<TableInfo> tables = new List<TableInfo>();
            if (connection is SqlConnection || connection is OleDbConnection)
            {
                DataTable dt;
                if (tableOwner == null)
                    dt = connection.GetSchema("Tables");
                else
                    dt = connection.GetSchema("Tables", new string[2] { null, tableOwner });
                if (connection is SqlConnection)
                    foreach (DataRow row in dt.Rows)
                        tables.Add(TableInfo.FromSqlSchemaRow(row));
                else if (connection is OleDbConnection)
                {
                    foreach (DataRow row in dt.Rows)
                        tables.Add(TableInfo.FromOleDbSchemaRow(row));
                }
            }
            else if (connection is OracleConnection)
            {
                DataTable dt;
                if (tableOwner == null)
                    dt = connection.GetSchema("Tables");
                else
                    dt = connection.GetSchema("Tables", new string[1] { tableOwner });
                if (connection is OracleConnection)
                {
                    foreach (DataRow row in dt.Rows)
                        tables.Add(TableInfo.FromOracleSchemaRow(row));
                }
            }
            if (readColumns)
                foreach (TableInfo ti in tables)
                    ReadTableColumns(connection, ti);
            TableInfo[] array = new TableInfo[tables.Count];
            tables.CopyTo(array);
            Helper.CloseConnection(connection, close);
            return array;
        }

        public static TableInfo GetTable(string tableName)
        {
            return GetTable(null, tableName);
        }

        public static TableInfo GetTable(string tableOwner, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Parameter 'tableName' is not specified.");
            //not using ConnectionManager.instance.Connection becausse acourding to MSDN:
            //If the connection is associated with a transaction, executing GetSchema calls may cause some providers to throw an exception.
            DbConnection connection = ConnectionManager.DbProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionManager.ConnectionString;
            connection.Open();
            TableInfo table = null;
            if (connection is SqlConnection || connection is OleDbConnection)
            {
                DataTable dt = connection.GetSchema("Tables", new string[] { null, tableOwner, tableName });
                if (dt.Rows.Count > 0)
                {
                    if (connection is SqlConnection)
                        table = TableInfo.FromSqlSchemaRow(dt.Rows[0]);
                    else if (connection is OleDbConnection)
                        table = TableInfo.FromOleDbSchemaRow(dt.Rows[0]);
                }
                else
                    return null;
                //throw new Exception(string.Format("Table '{0}.{1}' does not exist.", tableOwner, tableName));
            }
            else if (connection is OracleConnection)
            {
                DataTable dt;
                dt = connection.GetSchema("Tables", new string[] { tableOwner, tableName });
                if (dt.Rows.Count > 0)
                {
                    //if ( connection is OracleConnection )
                    table = TableInfo.FromOracleSchemaRow(dt.Rows[0]);
                }
                else
                {
                    ConnectionManager.Instance.ReleaseConnection();
                    throw new Exception(string.Format("Table '{0}.{1}' does not exist.", tableOwner, tableName));
                }
            }
            ReadTableColumns(connection, table);
            //Helper.CloseConnection(connection, close);
            //ConnectionManager.Instance.ReleaseConnection();
            connection.Close();
            return table;
        }

        public static void ReadTableColumns(DbConnection connection, TableInfo table)
        {
            //bool close = false;
            //if (connection.State == ConnectionState.Closed)
            //{
            //    connection.Open();
            //    close = true;
            //}
            DataTable dt;
            if (connection is SqlConnection)
            {
                SqlCommand cmd = new SqlCommand("sp_columns", (SqlConnection)connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@table_name", table.TableName));
                if (!string.IsNullOrEmpty(table.TableOwner))
                    cmd.Parameters.Add(new SqlParameter("@table_owner", table.TableOwner));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                //dt = connection.GetSchema("Columns", new string[3] { null, table.TableOwner, table.TableName });
                foreach (DataRow row in dt.Rows)
                    table.Columns.Add(ColumnInfo.FromSqlSpColumnsRow(row));
                //    table.Columns.Add(ColumnInfo.FromSqlSchemaRow(row));
                string[] pkeys = GetSqlPrimeryKeys(table);
                foreach (var c in table.Columns)
                {
                    //    DataRow dr = DBDataUtility.GetColumnSetting(c.Table.TableName, c.ColumnName);
                    //    if (dr != null)
                    //    {
                    //        c.ColumnMode = ColumnInfo.GetColumnMode(dr["mode"].ToString());
                    //        c.description =dr["description"].ToString();
                    //        c.dontShow = (bool)dr["dontShow"];
                    //        c.readOnly = (bool)dr["readOnly"];
                    //        c.RadioColumnCount =(dr["RadioRepeatColumns"].ToString()=="" ? -1 : int.Parse( dr["RadioRepeatColumns"].ToString())) ;
                    //        c.ListValue = dr["ListValue"].ToString();
                    //        c.panelId = dr["panelId"].ToString();
                    //        c.panelName = dr["panelName"].ToString();

                    //    }

                    if (Array.IndexOf(pkeys, c.ColumnName) >= 0)
                        c.IsPrimaryKey = true;
                }
            }
            else if (connection is OracleConnection)
            {
                dt = connection.GetSchema("Columns", new string[2] { table.TableOwner, table.TableName });
                foreach (DataRow row in dt.Rows)
                    table.Columns.Add(ColumnInfo.FromOracleSchemaRow(row));
            }
            else if (connection is OleDbConnection)
            {
                //TODO: restrictions
                dt = connection.GetSchema("Columns", new string[2] { table.TableOwner, table.TableName });
                foreach (DataRow row in dt.Rows)
                    table.Columns.Add(ColumnInfo.FromOleDbSchemaRow(row));
            }
            //ColumnInfo[] array = new ColumnInfo[columns.Count];
            //columns.CopyTo(array);
            //table.Columns = array;
            //if (close)
            //    connection.Close();
            //return array;
        }

        public static string GenerateCreateTableStatement(TableInfo ti, DatabaseType databaseType)
        {
            string ret = "CREATE TABLE ";
            if (!string.IsNullOrEmpty(ti.TableOwner))
                ret += "\"" + ti.TableOwner + "\"";
            ret += ".\"" + ti.TableName + "\" (";
            for (int i = 0; i < ti.Columns.Count; i++)
            {
                ret += "\r\n\t" + ti.Columns[i].GetColumnDefinition(databaseType);
                if (i < ti.Columns.Count - 1)
                    ret += ",";
            }
            ret += " );\r\n";
            return ret;
        }

        public static DatabaseType GetDatabaseType(DbConnection connection)
        {
            if (connection is SqlConnection)
                return DatabaseType.SQLServer;
            else if (connection is OracleConnection)
                return DatabaseType.Oracle;
            //else if ( connection is OleDbConnection )
            return DatabaseType.OleDb;
        }
        public static void CreateTableOrSyncSchema(TableInfo table)
        {
            TableInfo dbTable = GetTable(table.TableOwner, table.TableName);
            DatabaseType dbType = ConnectionManager.DatabaseType;
            //DbCommand command = SqlHelper.CreateCommand();
            //command.CommandType = CommandType.Text;
            //bool close = Helper.OpenConnection(connection);
            if (dbTable == null)
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    //try
                    //{
                    //command.Transaction = command.Connection.BeginTransaction();
                    SqlHelper.ExecuteNonQuery(GenerateCreateTableStatement(table, dbType));
                    if (table.ForeignKeys.Count > 0)
                    {
                        SqlHelper.ExecuteNonQuery(GenerateCreateForeignKeysStatement(table.TableName, table.ForeignKeys));
                    }
                    tran.Complete();
                    //command.Transaction.Commit();
                    //}
                    //catch (SqlException)
                    //{
                    //    //if (command.Transaction != null)
                    //    //    command.Transaction.Rollback();
                    //    throw;
                    //}
                }
            }
            else
            {
                List<ColumnInfo> alter = new List<ColumnInfo>();
                List<ColumnInfo> add = new List<ColumnInfo>();
                foreach (ColumnInfo column in table.Columns)
                {
                    ColumnInfo dbColumn = null;
                    foreach (ColumnInfo ci in dbTable.Columns)
                        if (ci.ColumnName == column.ColumnName)
                        {
                            dbColumn = ci;
                            break;
                        }
                    if (dbColumn == null)
                        add.Add(column);
                    else if (column != dbColumn)
                        alter.Add(column);
                }
                string alterTable = "ALTER TABLE " + table.FullName + " ";
                StringBuilder sb = new StringBuilder();
                if (add.Count > 0)
                {
                    sb.Append(alterTable).Append("ADD ");
                    bool b = true;
                    foreach (ColumnInfo ci in add)
                    {
                        if (b)
                            b = false;
                        else
                            sb.Append(", ");
                        sb.Append(ci.GetColumnDefinition(dbType));
                    }
                    sb.Append(Environment.NewLine);
                }
                foreach (ColumnInfo ci in alter)
                    sb.Append(alterTable).Append("ALTER COLUMN ").Append(ci.GetColumnDefinition(dbType)).Append(Environment.NewLine);

                dbTable.ForeignKeys.Load();
                foreach (ColumnInfo column in dbTable.Columns)
                {
                    bool found = false;
                    foreach (ColumnInfo ci in table.Columns)
                        if (ci.ColumnName == column.ColumnName)
                        {
                            found = true;
                            break;
                        }
                    if (!found)
                    {
                        string cn = GetDefaultConstraint(table.TableName, column.ColumnName);
                        if( cn != null )
                            sb.Append(alterTable).Append("DROP CONSTRAINT ").Append(cn).Append(Environment.NewLine);
                        var fk = dbTable.ForeignKeys.FirstOrDefault(ff => ff.FKField == column.ColumnName);
                        if( fk != null)
                            sb.Append(alterTable).Append("DROP CONSTRAINT ").Append(fk.Name).Append(Environment.NewLine);
                        sb.Append(alterTable).Append("DROP COLUMN ").Append(column.ColumnName).Append(Environment.NewLine);
                    }
                }
                List<ForeignKeyRelation> fks = new List<ForeignKeyRelation>();
                foreach (var fk in table.ForeignKeys)
                {
                    bool found = false;
                    foreach (ForeignKeyRelation dbfk in dbTable.ForeignKeys)
                        if (fk == dbfk)
                        {
                            found = true;
                            break;
                        }
                    if (!found)
                        fks.Add(fk);
                }
                foreach (ForeignKeyRelation dbfk in dbTable.ForeignKeys)
                {
                    bool found = false;
                    foreach (ForeignKeyRelation fk in table.ForeignKeys)
                        if (fk == dbfk)
                        {
                            found = true;
                            break;
                        }
                    if (!found)
                        sb.Append(alterTable).Append("DROP CONSTRAINT ").Append(dbfk.Name).Append(Environment.NewLine);
                }

                //try
                //{
                //    command.Transaction = command.Connection.BeginTransaction();
                using (TransactionScope tran = new TransactionScope())
                {
                    if (sb.Length > 0)
                    {
                        //command.CommandText = sb.ToString();
                        SqlHelper.ExecuteNonQuery(sb.ToString());
                        //command.ExecuteNonQuery();
                    }
                    if (fks.Count > 0)
                    {
                        SqlHelper.ExecuteNonQuery(GenerateCreateForeignKeysStatement(table.TableName, fks));
                    }
                    //    command.Transaction.Commit();
                    //}
                    //catch
                    //{
                    //    command.Transaction.Rollback();
                    //    throw;
                    //}
                    tran.Complete();
                }
            }
            //Helper.CloseConnection(connection, close);
        }

        private static string GetDefaultConstraint(string table, string column)
        {
            var o = SqlHelper.ExecuteScaler(@"SELECT df.name cn FROM sys.default_constraints df
INNER JOIN sys.tables t ON df.parent_object_id = t.object_id
INNER JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id where t.name='"
                + table + "' and c.NAME='" + column + "'");
            if (o != null && o != DBNull.Value)
                return o.ToString();
            return null;
        }

        public static void CreateForeignKeys(string tableName,
            List<ForeignKeyRelation> foreignKeys)
        {
            SqlHelper.ExecuteNonQuery(SchemaUtility.GenerateCreateForeignKeysStatement(tableName, foreignKeys));
        }
        public static string GenerateCreateForeignKeysStatement(string tableName,
            List<ForeignKeyRelation> foreignKeys)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var fk in foreignKeys)
            {
                sb.Append("ALTER TABLE ").Append(tableName).Append(" ADD CONSTRAINT FK_")
                    .Append(tableName).Append("_").Append(fk.FKField).Append("_").Append(fk.PKTable).Append(" FOREIGN KEY (")
                    .Append(fk.FKField).Append(") REFERENCES ").Append(fk.PKTable).Append(" (")
                    .Append(fk.PKField).Append(");").Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        public static string[] GetSqlPrimeryKeys(TableInfo table)
        {
            DataReader dr = null;
            List<string> pkeys = new List<string>();
            try
            {
                string strSelect = "exec sp_pkeys N'" + table.TableName + "',N'" + table.TableOwner + "'";
                dr = SqlHelper.ExecuteReader(strSelect);
                while (dr.Read())
                {
                    pkeys.Add(dr["Column_Name"].ToString());
                }
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return pkeys.ToArray();
        }
        public static string GetSqlColumnDescription(TableInfo table, ColumnInfo column)
        {
            string strDesc = "select isnull([value],'') from fn_listextendedproperty('MS_Description','schema','" + table.TableOwner + "','table',N'" + table.TableName + "','column',null) where objname = N'" + column.ColumnName + "';";
            return (string)SqlHelper.ExecuteScaler(strDesc) ?? "";
        }
        public static string GetSqlTableDescription(TableInfo table)
        {
            return GetSqlTableDescription(table.TableName, table.TableOwner);
        }
        public static string GetSqlTableDescription(string tableName, string tableOwner)
        {
            string strSelect = "select [value] from fn_listextendedproperty('MS_Description','schema','" + tableOwner + "','table',N'" + tableName + "',null,null)";
            return SqlHelper.ExecuteScaler(strSelect) as string;
        }

        public static void ReadDescriptions(TableInfo table)
        {
            table.Title = GetSqlTableDescription(table);
            foreach (ColumnInfo ci in table.Columns)
            {
                ci.Title = GetSqlColumnDescription(table, ci);
            }
        }

        public static ForeignKeyRelation[] GetForeignKeyRelations(string tableName)
        {
            List<ForeignKeyRelation> realations = new List<ForeignKeyRelation>();
            List<string> columns = new List<string>();
            DataReader dr = null;
            try
            {
                dr = SqlHelper.ExecuteReader("sp_fkeys @fktable_name= " + tableName);
                while (dr.Read())
                {
                    ForeignKeyRelation r = new ForeignKeyRelation();
                    r.FKField = dr["FKColumn_Name"].ToString();
                    r.PKTable = dr["PKTable_Name"].ToString();
                    r.PKField = dr["PKColumn_Name"].ToString();
                    r.Name = dr["FK_NAME"].ToString();
                    realations.Add(r);
                    //string fKtableName = dr["FKTable_Name"].ToString();
                    //innerSelects.Add(fKColumnName.ToLower(), "( Select [Name] From " + "dbo" + "." + pKtableName + " Where " + pKtableName + ".[" + pKColumnName + "] =  " + fKtableName + ".[" + fKColumnName + "]  ) ");
                }
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return realations.ToArray();
        }

        public static void DeleteForeignKey(string tableName, string fkName)
        {
            SqlHelper.ExecuteNonQuery(string.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", tableName, fkName));
        }

        #region Helper class

        private static class Helper
        {
            public static bool OpenConnection(DbConnection connection)
            {
                bool close = false;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    close = true;
                }
                return close;
            }
            public static void CloseConnection(DbConnection connection, bool close)
            {
                if (close)
                    connection.Close();
            }
        }

        #endregion Helper class


    }


    public enum DatabaseType
    {
        SQLServer,
        Oracle,
        OleDb,
        SQLite
    }
}

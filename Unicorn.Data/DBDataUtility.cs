using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Linq;

namespace Unicorn.Data
{
    public static class DBDataUtility
    {
        public static string GetInsertStatements(DbConnection connection, TableInfo table)
        {
            return GetInsertStatements(connection, table.FullName);
        }
        public static string GetInsertStatements(DbConnection connection, string tableName)
        {
            bool close = false;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                close = true;
            }

            DbCommand cmd = GetCommand("select * from [" + tableName + "]", connection);
            DbDataReader dr = cmd.ExecuteReader();
            StringBuilder sb = new StringBuilder();
            while (dr.Read())
            {
                sb.Append(GetRowInsertStatement(dr, tableName)).Append("\r\n");
            }
            dr.Close();

            if (close)
                connection.Close();
            return sb.ToString();
        }

        public static void WriteInsertStatements(DbConnection connection, string tableName, StreamWriter streamToWrite)
        {
            bool close = false;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                close = true;
            }

            DbCommand cmd = GetCommand("select * from [" + tableName + "]", connection);
            DbDataReader dr = cmd.ExecuteReader();
            //StreamWriter sw = new StreamWriter( streamToWrite, System.Text.Encoding.Unicode );
            while (dr.Read())
            {
                streamToWrite.WriteLine(GetRowInsertStatement(dr, tableName));
            }
            //sw.Close();
            dr.Close();

            if (close)
                connection.Close();
        }

        internal static string GetRowInsertStatement(DbDataReader dr, string tableName)
        {
            int n = dr.FieldCount;
            StringBuilder sb = new StringBuilder("insert into [" + tableName + "] values (");
            bool b = true;
            for (int i = 0; i < n; i++)
            {
                if (b)
                    b = false;
                else
                    sb.Append(",");
                if (dr[i] is DBNull)
                    sb.Append("NULL");
                else if (dr[i] is string)
                    sb.Append("N'").Append(dr[i].ToString().Replace("\r\n", "\\r\\n").Replace("'", "''")).Append("'");
                else
                    sb.Append(dr[i].ToString());
            }
            sb.Append(");");
            return sb.ToString();
        }

        private static DbCommand GetCommand(string sqlCommand, DbConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sqlCommand;
            return cmd;
            //if (connection is DbConnection)
            //    return SqlHelper.CreateCommand(sqlCommand, (DbConnection)connection);
            //else if (connection is OracleConnection)
            //    return new OracleCommand(sqlCommand, (OracleConnection)connection);
            //else // if( connection is OleDbConnection)
            //    return new OleDbCommand(sqlCommand, (OleDbConnection)connection);
        }
        public static string GetPagedSelectStatement(TableInfo table, string sortExpression, int startIndex, int maximumRows, string condition)
        {
            string sel = "SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY {1}) AS _RowNumber FROM {0} WHERE 1=1 " + condition + " ) AS TableWithRowNumber WHERE _RowNumber > {2} AND _RowNumber <= {3}" + condition;
            return string.Format(sel, table.FullName, sortExpression, startIndex, startIndex + maximumRows);
        }
        public static string GetPagedSelectStatement(string tableName, string fieldList, string sortExpression
            , string whereClause, int startIndex, int maximumRows)
        {
            string inner = string.Format("SELECT {0}, ROW_NUMBER() OVER(ORDER BY {1}) AS _RowNumber FROM {2}"
                , fieldList, sortExpression, tableName);
            if (!string.IsNullOrEmpty(whereClause))
                inner += " where " + whereClause;
            string sel = "SELECT * FROM ( {0} ) AS TableWithRowNumber WHERE _RowNumber > {1} AND _RowNumber <= {2}";
            return string.Format(sel, inner, startIndex, startIndex + maximumRows);
        }
        public static int GetRowIndex(TableInfo table, string sortExpression, string keyFieldName, object keyFieldValue)
        {
            string sel = "SELECT ROW_NUMBER() OVER(ORDER BY {1}) AS _RowNumber FROM {0} WHERE {2} = {3}";
            sel = string.Format(sel, table.FullName, sortExpression, keyFieldName, keyFieldValue);
            return (int)SqlHelper.ExecuteScaler(sel);
        }

        public static bool IsColumnInTable(string columnName, string TableName)
        {
            DataReader dr = null;
            bool isExist = false;
            try
            {
                string str = "sp_columns @table_name= N'" + TableName + "',@column_name= N'" + columnName + "'";
                dr = SqlHelper.ExecuteReader(str);
                isExist = dr.HasRows;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return isExist;
        }

        public static string GetColumnType(string columnName, string TableName)
        {
            DataReader dr = null;
            string colType = "";
            try
            {
                string str = "sp_columns @table_name = N'" + TableName + "' , @column_name = N'" + columnName + "'";
                dr = SqlHelper.ExecuteReader(str);
                if (!dr.HasRows)
                    colType = null;
                else
                {
                    dr.Read();
                    colType = dr["Type_Name"].ToString();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return colType;
        }

        //public static bool IsColumnNullable(string columnName, string TableName)
        //{
        //    DbDataReader dr = null;
        //    bool nullable = false;
        //    try
        //    {
        //        string str = "sp_columns @table_name = N'" + TableName + "' , @column_name = N'" + columnName + "'";
        //        dr = SqlHelper.ExecuteReader(str);
        //        dr.Read();
        //        nullable = (dr["Nullable"].ToString()== 1);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (dr != null)
        //            dr.Close();
        //    }
        //    return nullable;
        //}

        public static string[] GetPrimeryFields(string tableName)
        {
            DataReader dr = null;
            List<string> pkeys = new List<string>();
            try
            {
                string strSelect = "exec sp_pkeys N'" + tableName + "' ";
                dr = SqlHelper.ExecuteReader(strSelect);
                while (dr.Read())
                {
                    pkeys.Add(dr["Column_Name"].ToString());
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return pkeys.ToArray();
        }

        public static string[] GetColumns(string tableName)
        {
            List<string> columns = new List<string>();
            string strSelect = "sp_columns N'" + tableName + "'";
            DataReader dr = null;
            try
            {
                dr = SqlHelper.ExecuteReader(strSelect);
                while (dr.Read())
                    columns.Add(dr["Column_Name"].ToString());
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return columns.ToArray();
        }

        public static string[] GetColumnsFromSelectCommand(string selectCommand)
        {
            List<string> columns = new List<string>();
            DataReader dr = null;
            try
            {
                selectCommand = @"
Set FmtOnly  On
" + selectCommand + @"
Set FmtOnly  Off";
                dr = SqlHelper.ExecuteReader(selectCommand);
                int fieldCount = dr.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                    //            while (dr.Read())
                    columns.Add(dr.GetName(i));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return columns.ToArray();
        }

        public static string[] GetSqlDataType()
        {
            DataReader dr = null;
            List<string> dtypes = new List<string>();
            try
            {
                string strSelect = "exec sp_datatype_info_100";
                dr = SqlHelper.ExecuteReader(strSelect);
                while (dr.Read())
                {
                    dtypes.Add(dr["Type_Name"].ToString());
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return dtypes.ToArray();
        }

        public static string[] GetColumnsTypeFromSelectCommand(string selectCommand)
        {
            List<string> columnsType = new List<string>();
            DataReader dr = null;
            try
            {
                selectCommand = @"
Set FmtOnly  On
" + selectCommand + @"
Set FmtOnly  Off";
                dr = SqlHelper.ExecuteReader(selectCommand);
                int fieldCount = dr.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                    columnsType.Add(dr.GetDataTypeName(i));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return columnsType.ToArray();
        }

        public static string[] GetForeignKeys(string tableName)
        {
            List<string> foreignKeys = new List<string>();
            string strSelect = "sp_fkeys @fktable_name = N'" + tableName + "' ";
            DataReader dr = null;
            try
            {
                dr = SqlHelper.ExecuteReader(strSelect);
                while (dr.Read())
                    foreignKeys.Add(dr["FKColumn_Name"].ToString());
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return foreignKeys.ToArray();
        }

        public static string GetColumnDescription(string columnName, string tableName)
        {
            string desc = "";
            DataReader dr = null;
            try
            {
                string strDesc = "select isnull([value],'') from fn_listextendedproperty('MS_Description','schema','dbo','table',N'" + tableName + "','column',null) where objname = N'" + columnName + "';";
                //strDesc = Unicorn.ControlUtility.ReplaceDBOwner(strDesc).Replace("'dbo'", "'" + "dbo" + "'");
                //replace دوم در خط بالا بايد در داخل متد قرارگيرد.
                dr = SqlHelper.ExecuteReader(strDesc);
                if (!dr.HasRows)
                    desc = "";
                else
                {
                    dr.Read();
                    desc = dr[0].ToString();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return desc;
        }

        public static string AddTopToSelectComand(string selectCommand)
        {
            selectCommand = selectCommand.TrimStart();
            char[] chars = { '\n', '\r', ' ' };
            if (!selectCommand.TrimStart(chars).StartsWith("select", StringComparison.OrdinalIgnoreCase))
                throw new Exception("كوئري مورد نظر حتما بايد با عبارت Select آغاز گردد.");
            string selectClause = "Select";
            if (selectCommand.StartsWith("select distinct", StringComparison.OrdinalIgnoreCase))
                selectClause = "Select Distinct";
            if (selectCommand.TrimStart().StartsWith(selectClause + " top", StringComparison.OrdinalIgnoreCase))
                return selectCommand;
            //int rowCount = SqlHelper.ExecuteScaler("select Count(*) From " + TableName).ToInt();
            return selectClause + " Top 10000000000 " + selectCommand.Substring(selectClause.Length + 1);
        }

        public static string GetTableDescription(string tableName)
        {
            string strSelect = "select [value] from fn_listextendedproperty('MS_Description','schema','" + "dbo" + "','table',N'" + tableName + "',null,null)";
            return SqlHelper.ExecuteScaler(strSelect) as string;
        }

        public static bool AddTable(string tbl, string description)
        {
            if (!CheckTableExist(tbl))
            {
                string primaryKey = tbl + "id";
                SqlHelper.ExecuteNonQuery(@"CREATE TABLE [" + tbl + @"]
                        (
                        [" + primaryKey + @"] int NOT NULL IDENTITY (1, 1)
                        )  ON [PRIMARY]");

                AddTableDescription(tbl, description);
                AddFieldDescription(tbl, primaryKey, "کد");
                SqlHelper.ExecuteNonQuery(@"ALTER TABLE [" + tbl + @"] ADD CONSTRAINT
	                [PK_" + tbl + @"] PRIMARY KEY CLUSTERED 
	                (
	                " + primaryKey + @"
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    ");

                return true;
            }
            else
                return false;
        }

        public static void AddTableDescription(string tbl, string description)
        {
            SqlHelper.ExecuteNonQuery("EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', @tbl, NULL, NULL",
                new SqlParameter("tbl", tbl),
                new SqlParameter("v", description));
        }

        public static void AddFieldDescription(string tbl, string colName, string colDescription)
        {
            SqlHelper.ExecuteNonQuery("EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', @tbl, N'COLUMN', @desc",
                new SqlParameter("tbl", tbl),
                new SqlParameter("desc", colName),
                new SqlParameter("v", colDescription));
        }

        public static bool AddFieldToTable(string tbl, string colName, string colDescription, string dataType)
        {
            return AddFieldToTable(tbl, colName, colDescription, dataType, true);
            /* if (!CheckColumnExistInTable(tbl, colName))
             {
                 SqlHelper.ExecuteNonQuery("ALTER TABLE " + tbl + " Add " + colName + " " + dataType + " Null");
                 AddFieldDescription(tbl, colName, colDescription);
                 return true;
             }
             else
                 return false;*/
        }

        public static bool AddFieldToTable(string tbl, string colName, string colDescription, string dataType, bool nullable)
        {
            if (!CheckColumnExistInTable(tbl, colName))
            {
                SqlHelper.ExecuteNonQuery("ALTER TABLE [" + tbl + "] Add [" + colName + "] " + dataType + " " + (nullable ? "Null" : "Not Null"));
                AddFieldDescription(tbl, colName, colDescription);
                return true;
            }
            else
                return false;
        }

        public static void AddRelationBetweenTables(string mainTbl, string subsidiaryTbl, string colName)
        {
            SqlHelper.ExecuteNonQuery(@"ALTER TABLE [" + subsidiaryTbl + @"] ADD CONSTRAINT
            FK_" + subsidiaryTbl + "_" + mainTbl + @" FOREIGN KEY
            (
            [" + colName + @"]
            ) REFERENCES [" + mainTbl + @"]
            (
            [" + GetPrimeryFields(mainTbl)[0] + @"]
            ) ON UPDATE  NO ACTION 
            ON DELETE  NO ACTION ");


        }
        public static bool CheckTableExist(string tbl)
        {
            return bool.Parse(SqlHelper.ExecuteScaler(@"IF EXISTS (SELECT 1 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE='BASE TABLE' 
                AND TABLE_NAME=@tbl  )  SELECT 'true' 
               ELSE 
                 SELECT 'false'  ",
                                        new SqlParameter("tbl", tbl)).ToString());
        }

        public static bool CheckColumnExistInTable(string tbl, string col)
        {
            return bool.Parse(SqlHelper.ExecuteScaler(@"IF EXISTS ( select * from sys.columns where Name = N'" + col + @"'  
            and Object_ID = Object_ID(N'" + tbl + @"'))
            select 'true'
            else select 'false' ").ToString());
        }

        public static void DetachDataBase(string dataBaseName)
        {
            if (ConnectionManager.DatabaseType != DatabaseType.SQLServer)
                return;
            SqlConnection.ClearAllPools();
            SqlHelper.ExecuteNonQuery("EXEC sp_detach_db [" + dataBaseName + "], 'true'");
        }

        public static void AttachDataBase(string dataBaseName, string mapPath)
        {
            if (ConnectionManager.DatabaseType != DatabaseType.SQLServer)
                return;
            SqlHelper.ExecuteNonQuery("EXEC sp_attach_db @dbname = N'" + dataBaseName + "',@filename1 = N'" + mapPath + "\\" + dataBaseName + ".mdf',@filename2 = N'" + mapPath + "\\" + dataBaseName + "_log.ldf'");
        }

        public static void CopyFromDataBase(string dataBaseName, string mapPath)
        {
            System.IO.File.Copy(mapPath + "\\" + dataBaseName + ".Mdf", mapPath + "\\" + dataBaseName + Unicorn.PersianDateTime.Now.Year + ".mdf");
            System.IO.File.Copy(mapPath + "\\" + dataBaseName + "_log.Ldf", mapPath + "\\" + dataBaseName + Unicorn.PersianDateTime.Now.Year + "_log.Ldf");

        }

        public static string[] GetIdentityFields(string tableName)
        {
            //TODO: other databases
            if (ConnectionManager.DatabaseType != DatabaseType.SQLServer)
                return null;
            string strCommand = "exec sp_columns N'" + tableName + "'";
            DataReader dr = null;
            try
            {
                List<string> identityFields = new List<string>();
                dr = SqlHelper.ExecuteReader(strCommand);
                while (dr.Read())
                    if (dr["Type_Name"].ToString() == "int identity")
                        identityFields.Add(dr["column_name"].ToString());
                return identityFields.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        public static string[] GetTables()
        {
            List<string> tables = new List<string>();
            DataReader dr = null;
            try
            {
                dr = SqlHelper.ExecuteReader("exec sp_tables @Table_Type =  \"'TABLE'\"");
                while (dr.Read())
                {
                    string table = dr["Table_NAME"].ToString();
                    if (table.Trim() != "dtproperties" && !table.Contains("aspnet_"))
                        tables.Add(table);
                }
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return tables.ToArray();
        }

        public static string[] GetViews()
        {
            List<string> views = new List<string>();
            DataReader dr = null;
            try
            {
                dr = SqlHelper.ExecuteReader("exec sp_tables @Table_Type =  \"'VIEW'\",@Table_Owner = 'dbo'");
                while (dr.Read())
                {
                    string table = dr["Table_NAME"].ToString();
                    //if (table.Trim() != "dtproperties" && !table.Contains("aspnet_"))
                    views.Add(table);
                }
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return views.ToArray();
        }

        public static int GetRecourdCountOfSelectCommand(string strSelect, params SqlParameter[] sqlParameters)
        {
            strSelect = @"
Select 
    Count(*)
From (
    " + AddTopToSelectComand(strSelect) + @"
    ) Temp";
            int n = (int)SqlHelper.ExecuteScaler(strSelect, sqlParameters);
            return n;
        }
        /// <summary>
        /// يك كوئريSelect شامل همه فيلد ها و نام كليد خارجي ها به نام مستعار فارسي بر ميگرداند.
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static string GetFullSelectCommand(string TableName, bool primaryFieldsInCommand, bool fieldDescAsAliasName, params string[] customColumns)
        {
            string selectCommand = "";
            Dictionary<string, string> foreignKeyInnerSelects = GetForeignKeyInnerSelects(TableName);
            List<string> columns = new List<string>();
            DataReader dr = null;
            string[] primaryKeys = Unicorn.Data.DBDataUtility.GetPrimeryFields(TableName);
            string[] foreignKeys = Unicorn.Data.DBDataUtility.GetForeignKeys(TableName);
            try
            {
                string strCommand = "sp_columns N'" + TableName + "'";
                dr = SqlHelper.ExecuteReader(strCommand);
                while (dr.Read())
                {
                    string columnName = dr["Column_Name"].ToString();
                    if (!primaryFieldsInCommand && primaryKeys.Contains(columnName))//, StringComparison.OrdinalIgnoreCase) != -1)
                        continue;

                    string str = "";
                    if (columnName.ToLower().Contains("date"))
                        str = "dbo.SetDate(" + TableName + ".[" + columnName + "]" + ",'/',0)";
                    else if (foreignKeyInnerSelects.ContainsKey(columnName.ToLower()))
                        str = foreignKeyInnerSelects[columnName.ToLower()];
                    else
                        str = TableName + ".[" + columnName + "]";
                    string columnAliasName = GetColumnAliasName(columnName, TableName, foreignKeys, fieldDescAsAliasName);
                    if (string.IsNullOrEmpty(columnAliasName))
                        throw new NotImplementedException();
                    str += " AS [" + columnAliasName + "]";
                    columns.Add(str);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            columns.AddRange(customColumns);
            if (columns.Count == 0)
                throw new Exception("هيچ فيلدي در جدول «" + TableName + "» يافت نشد.");
            selectCommand = @"
    Select 
        " + string.Join(",\r\n", columns.ToArray()) /*+ (this.AutoGenerateFields ? GetForeignKeysNameInnserSelect() : "")*/ + @" 
    From 
        " + TableName;
            return selectCommand;
        }

        private static string GetColumnAliasName(string columnName, string tableName, string[] foreignKeys, bool useFieldDescAsAliasName)
        {
            string columnAliasName = "";
            string columnDesc = Unicorn.Data.DBDataUtility.GetColumnDescription(columnName, tableName);
            if (columnDesc == "")
                columnDesc = columnName;
            if (useFieldDescAsAliasName)
                columnAliasName += columnDesc;
            else if (!foreignKeys.Contains(columnName))//, StringComparison.OrdinalIgnoreCase))
                columnAliasName += columnName;
            else
                columnAliasName += GetNameFieldOfForeignKey(columnName);
            return columnAliasName;
        }

        /// <summary>
        /// براي مثال: 
        /// PersonnelId -> PersonnelIdName
        /// </summary>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        private static string GetNameFieldOfForeignKey(string foreignKey)
        {
            return foreignKey + "Name";
        }


        private static Dictionary<string, string> GetForeignKeyInnerSelects(string TableName)
        {
            Dictionary<string, string> innerSelects = new Dictionary<string, string>();
            //string selectCommands = "";
            string strSelect = "sp_fkeys @fktable_name = N'" + TableName + "'";
            List<string> columns = new List<string>();
            DataReader dr = null;
            try
            {
                dr = SqlHelper.ExecuteReader(strSelect);
                while (dr.Read())
                {
                    string fKColumnName = dr["FKColumn_Name"].ToString();
                    string pKtableName = dr["PKTable_Name"].ToString();
                    string pKColumnName = dr["PKColumn_Name"].ToString();
                    string fKtableName = dr["FKTable_Name"].ToString();
                    if (innerSelects.ContainsKey(fKColumnName.ToLower()))
                        throw new Exception("فيلد «" + fKColumnName.ToLower() + "» قبلا به عنوان كليد خارجي در جدول  «" + fKColumnName.ToLower() + "» ثبت شده است و مجددا قابل ثبت نمي باشد.\r\nاحتمالا بيش از يك ارتباط روي اين فيلد تعريف شده است.");
                    innerSelects.Add(fKColumnName.ToLower(), "( Select [Name] From " + "dbo" + ".[" + pKtableName + "] Where [" + pKtableName + "].[" + pKColumnName + "] =  [" + fKtableName + "].[" + fKColumnName + "]  ) ");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return innerSelects;
        }

        public static bool IsValidCommand(string sqlCommand)
        {
            try
            {
                sqlCommand = @"Set FmtOnly  On
" + sqlCommand + @"
Set FmtOnly  Off";
                SqlHelper.ExecuteNonQuery(sqlCommand);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static KeyValuePair<string, string> GetRelation(string tableName, string fieldName)
        {
            KeyValuePair<string, string>? relation = null;
            string strSelect = "sp_fkeys @fktable_name = N'" + tableName + "'";
            List<string> columns = new List<string>();
            DataReader dr = null;
            try
            {
                dr = SqlHelper.ExecuteReader(strSelect);
                while (dr.Read())
                {
                    string fKColumnName = dr["FKColumn_Name"].ToString();
                    string pKtableName = dr["PKTable_Name"].ToString();
                    string pKColumnName = dr["PKColumn_Name"].ToString();
                    string fKtableName = dr["FKTable_Name"].ToString();
                    if (fKColumnName.ToLower() == fieldName.ToLower())
                    {
                        relation = new KeyValuePair<string, string>(pKtableName, pKColumnName);
                        break;
                    }
                }
                if (relation == null)
                    throw new Exception("هيچ Relationي روي فيلد «" + tableName + "." + fieldName + "» تعري نشده است.");
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return (KeyValuePair<string, string>)relation;
        }

        public static string[] GetRequiredFields(string tableName)
        {
            string strCommand = "exec sp_columns N'" + tableName + "'";
            DataReader dr = null;
            try
            {
                List<string> requiredFields = new List<string>();
                dr = SqlHelper.ExecuteReader(strCommand);
                while (dr.Read())
                    if (dr["NULLABLE"].ToString() == "0")
                        requiredFields.Add(dr["column_name"].ToString());
                return requiredFields.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        public static Dictionary<string, object>[] GetAllValues(string strSelect, params SqlParameter[] sqlParameters)
        {
            DataReader dr = null;
            List<Dictionary<string, object>> values = new List<Dictionary<string, object>>();
            try
            {
                dr = SqlHelper.ExecuteReader(strSelect, sqlParameters);
                while (dr.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int i = 0; i <= dr.FieldCount - 1; i++)
                        row.Add(dr.GetName(i), dr[i]);
                    values.Add(row);
                }
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return values.ToArray();
        }

        public static object[] GetValues(string strSelect, params SqlParameter[] sqlParameters)
        {
            DataReader dr = null;
            List<object> values = new List<object>();
            try
            {
                dr = SqlHelper.ExecuteReader(strSelect, sqlParameters);
                while (dr.Read())
                {
                    values.Add(dr[0]);
                }
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return values.ToArray();
        }

        public static string GetSqlServerVersion()
        {
            string version = "";
            try
            {
                DbConnection con = ConnectionManager.Instance.GetConnection(); ;
                version = con.ServerVersion;
            }
            finally
            {
                ConnectionManager.Instance.ReleaseConnection();
                //if (con != null)
                //    con.Close();
            }
            if (string.IsNullOrEmpty(version))
                throw new Exception("ورژن SqlServer يافت نشد.");
            return version;
        }

        public static string GetDatabaseName()
        {
            var name = ConnectionManager.Instance.GetConnection().Database;
            ConnectionManager.Instance.ReleaseConnection();
            return name;
        }

        public static int? GetColumnLength(string fieldName, string tableName)
        {
            string strSelect = "Select CHARACTER_MAXIMUM_LENGTH FRom INFORMATION_SCHEMA.COLUMNS Where COLUMN_NAME = @COLUMN_NAME And TABLE_NAME = @TABLE_NAME";
            SqlParameter prm1 = new SqlParameter("COLUMN_NAME", fieldName);
            SqlParameter prm2 = new SqlParameter("TABLE_NAME", tableName);
            object obj = SqlHelper.ExecuteScaler(strSelect, prm1, prm2);
            if (obj == null || obj == DBNull.Value)
                return null;
            return (int)obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="selectedFields">فيلدهاي انتخابي كه با ويرگول از هم جداشده اند</param>
        /// <param name="conditionField"></param>
        /// <param name="conditionFieldValue"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string tableName, string selectedFields, string conditionField, object conditionFieldValue)
        {
            selectedFields = "[" + string.Join("],[", selectedFields.Split(',')) + "]";
            string strSelect = "Select " + selectedFields + " From [" + tableName + "] Where [" + conditionField + "] = @" + conditionField;
            SqlParameter prm = new SqlParameter(conditionField, conditionFieldValue);
            return SqlHelper.ExecuteCommand(strSelect, prm);
        }

        public static object GetValue(string tableName, string selectedField, string conditionField, object conditionFieldValue)
        {
            string strSelect = "Select [" + selectedField + "] From [" + tableName + "] Where [" + conditionField + "] = @" + conditionField;
            SqlParameter prm = new SqlParameter(conditionField, conditionFieldValue);
            prm.SqlDbType = SqlDbType.NVarChar;
            object obj = SqlHelper.ExecuteScaler(strSelect, prm);
            return obj;
        }

        public static object GetValue(string tableName, string selectedField)
        {
            string strSelect = "Select top 1 [" + selectedField + "] From [" + tableName + "] ";
            object obj = SqlHelper.ExecuteScaler(strSelect);
            return obj;
        }

        public static DataTable GetTableInfo(string tableName)
        {
            string strCmd = @"
SELECT  
	Name = syscolumns.name,  
	[Desc] = (select ep.value from sys.extended_properties ep where Sysobjects.id = ep.major_id  and ep.minor_id = syscolumns.colid ), 
	[Type] = (select top 1 systypes.name from systypes where syscolumns.xtype=systypes.xtype ), 
	Size = convert(varchar(100),syscolumns.length), 
	IsKey = (case when (syscolumns.name in (SELECT column_name FROM INFORMATION_SCHEMA.key_column_usage WHERE TABLE_NAME=Sysobjects.name and constraint_name like'%pk%')) Then '1' end),	
	PrimaryTableName = primaryTable.name,
	PrimaryTableDesc = ( select value From sys.extended_properties Where major_id = primaryTable.id And minor_id = 0),
	--PrimaryTableDesc = ( select [value] from fn_listextendedproperty('MS_Description','schema','dbo','table',primaryTable.name,null,null) ),
    PrimaryFieldName = primaryField.name,
    PrimaryFieldDesc = ( select value From sys.extended_properties Where major_id = primaryTable.id And minor_id = primaryField.colid),
	--PrimaryFieldDesc = ( select isnull([value],'') from fn_listextendedproperty('MS_Description','schema','dbo','table',primaryTable.name,'column',null) where objname = primaryField.name ), 
    NameFieldOfPrimaryTable = nameFieldOfPrimaryTable.name ,
    NameFieldOfPrimaryTableDesc = ( select value From sys.extended_properties Where major_id = primaryTable.id And minor_id = NameFieldOfPrimaryTable.colid)
	--NameFieldOfPrimaryTableDesc = ( select isnull([value],'') from fn_listextendedproperty('MS_Description','schema','dbo','table',primaryTable.name,'column',null) where objname = NameFieldOfPrimaryTable.name )
FROM 
	Sysobjects 
	Join syscolumns On Sysobjects.id = syscolumns.id 
    left Join sysconstraints ON Sysobjects.id = sysconstraints.id AND sysconstraints.colid = syscolumns.colid
    left Join sysforeignkeys ON sysconstraints.constid = sysforeignkeys.constid
	left Join sysobjects primaryTable ON sysforeignkeys.rkeyid = primaryTable.id
    left Join syscolumns primaryField ON sysforeignkeys.rkey = primaryField.colid AND sysforeignkeys.rkeyid = primaryField.id
    left Join syscolumns nameFieldOfPrimaryTable ON 
		primaryTable.id = nameFieldOfPrimaryTable.id 
		And (
			Lower(nameFieldOfPrimaryTable.name) = 'name' 
			Or nameFieldOfPrimaryTable.colid = (Select MIN(cls.colid) + 1 From syscolumns cls Where cls.id = primaryTable.id) 
            /*فیلد Name یا فیلد دوم به عنوان فیلد نام در جدول Primary در نظرگرفته می شود.*/
		)
where   
	(Sysobjects.xtype='u' or Sysobjects.xtype='v') 
	and Sysobjects.name= @TableName
order by  
	syscolumns.colid
";
            SqlParameter prm = new SqlParameter("TableName", tableName);
            return SqlHelper.ExecuteCommand(strCmd, prm);
        }
    }


}

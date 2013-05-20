using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Data.Common;
using System.Linq;

namespace Unicorn.Data
{
    public class Record
    {
        #region Properties And Fields

        Dictionary<string, object> fields;
        private string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        Dictionary<string, object> keys;
        public Dictionary<string, object> Keys
        {
            get { return keys; }
            set { keys = value; }
        }
        public string[] FieldNames { get { return fields.Keys.ToArray(); } }
        //DbTransaction transaction;
        //public DbTransaction Transaction
        //{
        //    get { return transaction; }
        //    set { transaction = value; }
        //}
        //private object commitException;
        //public object CommitException
        //{
        //    get { return commitException; }
        //    set { commitException = value; }
        //}
        public string Filter { get; set; }

        #endregion Properties And Fields

        #region Public Methods

        /// <summary>
        /// فيلد و مقدار مورد نظر را مشخص نماييد.        
        /// </summary>
        /// <param name="filedName"></param>
        /// <returns></returns>
        public object this[string filedName]
        {
            get { return fields[filedName]; }
            set
            {
                Stream stream = null;
                if (value is Stream)
                    stream = (Stream)value;
                else if (value is HttpPostedFile)
                    stream = ((HttpPostedFile)value).InputStream;
                if (stream != null)
                {
                    int length = Convert.ToInt32(stream.Length);
                    byte[] byte_array = new byte[length];
                    stream.Read(byte_array, 0, length);
                    fields[filedName] = byte_array;
                }
                else
                    fields[filedName] = value;
            }
        }

        public Record()
        {
            fields = new Dictionary<string, object>();
            keys = new Dictionary<string, object>();
            //transaction = null;
        }

        public Record(string tableName)
            : this()
        {
            this.tableName = tableName;
        }

        public Record Insert(bool isIdentityInsertOn)
        {
            //try
            //{
            string strInsert = GetInsertCommand();
            if (isIdentityInsertOn)
                AddAllowIdentityInsertToCommand(ref strInsert);
            SqlHelper.ExecuteNonQuery(strInsert, GetParameters(fields));
            //}
            //catch
            //{
            //    if (transaction != null)
            //        transaction.Rollback();
            //    throw;
            //}

            return ReturnNew();
        }

        public Record Insert()
        {
            return Insert(false);
        }

        public Record Update()
        {
            if (keys.Count == 0)
                throw new InvalidOperationException("No primary key has been specified for Update statement of table'" + tableName + "'");
            DbParameter[] fieldParameters = GetParameters(fields);
            DbParameter[] keyParameters = GetParameters(keys, "key_");
            DbParameter[] pars = new DbParameter[fieldParameters.Length + keyParameters.Length];
            Array.Copy(fieldParameters, 0, pars, 0, fieldParameters.Length);
            Array.Copy(keyParameters, 0, pars, fieldParameters.Length, keyParameters.Length);
            string updateCommand = String.Format("UPDATE {0} SET {1} WHERE {2}", tableName,
                string.Join(",", GetFieldsAndParamsPair(fields)),
                string.Join(" AND ", GetFieldsAndParamsPair(keys, "key_")));
            //try
            //{
            //AddAllowIdentityInsertToCommand(ref updateCommand);
            SqlHelper.ExecuteNonQuery(updateCommand, pars);
            //}
            //catch
            //{
            //    if (transaction != null)
            //        transaction.Rollback();
            //    throw;
            //}
            return ReturnNew();
        }

        public Record Delete()
        {
            string deleteCommand = String.Format("DELETE {0} WHERE {1}", tableName,
                string.Join(" AND ", GetFieldsAndParamsPair(keys)));
            int i = 0;
            //try
            //{
            i = SqlHelper.ExecuteNonQuery(deleteCommand, GetParameters(keys));
            //}
            //catch
            //{
            //    if (transaction != null)
            //        transaction.Rollback();
            //    throw;
            //}
            return ReturnNew();
        }

        //public void BeginTransaction()
        //{
        //    if (transaction != null)
        //        throw new InvalidOperationException("There is allready an open transaction in proccess");
        //    DbConnection con = ConnectionManager.Connection;
        //    transaction = con.BeginTransaction();
        //    commitException = null;
        //}

        //public bool Commit()
        //{
        //    if (transaction == null)
        //        throw new InvalidOperationException("كد خطا Jco.Web : 1390/12/16-15:54 : There is no transaction to commit.");
        //    CommitException = null;
        //    DbConnection connection = ConnectionManager.Connection;
        //    //try
        //    //{
        //    transaction.Commit();
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    commitException = e;
        //    //    try
        //    //    {
        //    //        transaction.Rollback();
        //    //    }
        //    //    catch
        //    //    {
        //    //    }
        //    //}
        //    transaction = null;
        //    return commitException == null;
        //}

        //public void Rollback()
        //{
        //    if (transaction == null)
        //        throw new InvalidOperationException("There is no transaction to rollback.");
        //    DbConnection connection = ConnectionManager.Connection;
        //    //try
        //    //{
        //    transaction.Rollback();
        //    //}
        //    //catch
        //    //{
        //    //}
        //    if (connection != null)
        //        connection.Close();
        //    transaction = null;
        //}

        #endregion Public Methods

        #region Private Methods

        private Record ReturnNew()
        {
            //if (transaction == null)
            //    return null;
            //else
            //{
                Record r = new Record(tableName);
                //r.transaction = transaction;
                return r;
            //}
        }

        private string GetInsertCommand()
        {
            string[] fieldNames = new string[fields.Count];
            string[] fieldValues = new string[fields.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> field in fields)
            {
                fieldNames[i] = "[" + field.Key + "]";
                if (field.Value == null || field.Value.ToString().Trim() == "")
                    fieldValues[i] = "null";
                else
                    fieldValues[i] = "@" + field.Key;
                i++;
            }
            return String.Format("Insert into {0} ({1}) VALUES ({2})", tableName,
                       string.Join(",", fieldNames), string.Join(",", fieldValues));
        }

        private DbParameter[] GetParameters()
        {
            return GetParameters(fields);
        }

        private DbParameter[] GetParameters(Dictionary<string, object> fieldValuePairs)
        {
            return GetParameters(fieldValuePairs, "");
        }

        private DbParameter[] GetParameters(Dictionary<string, object> fieldValuePairs, string parameterPrefix)
        {
            List<DbParameter> pars = new List<DbParameter>();
            foreach (KeyValuePair<string, object> field in fieldValuePairs)
                if (field.Value != null && field.Value.ToString().Trim() != "")
                    pars.Add(SqlHelper.CreateParameter("@" + parameterPrefix + field.Key, field.Value));


            return pars.ToArray();
        }

        private string[] GetFieldsAndParamsPair(Dictionary<string, object> fieldValuePairs)
        {
            return GetFieldsAndParamsPair(fieldValuePairs, "");
        }

        private string[] GetFieldsAndParamsPair(Dictionary<string, object> fieldValuePairs, string parameterPrefix)
        {
            string[] fieldAndValuesStr = new string[fieldValuePairs.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> fieldValuePair in fieldValuePairs)
            {
                string value = "";
                if (fieldValuePair.Value == null || fieldValuePair.Value.ToString().Trim() == "")
                    value = "null";
                else
                    value = "@" + parameterPrefix + fieldValuePair.Key;
                fieldAndValuesStr[i] = "[" + fieldValuePair.Key + "] = " + value;
                i++;
            }
            return fieldAndValuesStr;
        }

        private void AddAllowIdentityInsertToCommand(ref string strCommand)
        {
            strCommand = @"
SET IDENTITY_INSERT [" + TableName + @"] ON
" + strCommand + @"
Set identity_insert [" + TableName + @"] OFF;";
        }

        #endregion Private Methods

        //#region Static Methods

        //public static Record BeginTransaction(string tableName)
        //{
        //    DbConnection con = ConnectionManager.Connection;
        //    DbTransaction tran = con.BeginTransaction();
        //    Record r = new Record(tableName);
        //    r.transaction = tran;
        //    return r;
        //}

        //#endregion
    }
}

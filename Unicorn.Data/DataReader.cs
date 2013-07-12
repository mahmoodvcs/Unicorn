using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;


namespace Unicorn.Data
{
    public class DataReader:IDataReader, IEnumerable
    {
        private DbDataReader dr;
        internal DataReader(DbDataReader dr)
        {
            this.dr = dr;
        }
        #region IDataReader Members

        public void Close()
        {
            dr.Close();
            ConnectionManager.Instance.ReleaseConnection();
        }

        public int Depth
        {
            get { return dr.Depth; }
        }

        public DataTable GetSchemaTable()
        {
            return dr.GetSchemaTable();
        }

        public bool IsClosed
        {
            get { return dr.IsClosed; }
        }

        public bool NextResult()
        {
            return dr.NextResult();
        }

        public bool Read()
        {
            return dr.Read();
        }

        public int RecordsAffected
        {
            get { return dr.RecordsAffected; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!dr.IsClosed)
            {
                dr.Dispose();
                ConnectionManager.Instance.ReleaseConnection();
            }
            else
                dr.Dispose();
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get { return dr.FieldCount; }
        }

        public bool GetBoolean(int i)
        {
            return dr.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return dr.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return dr.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return dr.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return dr.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return dr.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return dr.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return dr.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return dr.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return dr.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return dr.GetFieldType(i); ;
        }

        public float GetFloat(int i)
        {
            return dr.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return dr.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return dr.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return dr.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return dr.GetInt64(i);
        }

        public string GetName(int i)
        {
            return dr.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return dr.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return dr.GetString(i);
        }

        public object GetValue(int i)
        {
            return dr.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return dr.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return dr.IsDBNull(i);
        }

        public object this[string name]
        {
            get
            {
                var o = dr[name];
                if (o is DBNull)
                    return null;
                return o;
            }
        }

        public object this[int i]
        {
            get { return dr[i]; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return dr.GetEnumerator();
        }

        #endregion

        public bool HasRows
        {
            get { return dr.HasRows; }
        }
    }
}

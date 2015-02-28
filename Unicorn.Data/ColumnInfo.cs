using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace Unicorn.Data
{
    [Serializable]
    public class ColumnInfo: ICloneable
    {
        private string columnName;
        private string title;
        private DbType dataType;
        //public string description;
        //public string panelId;
        //public string panelName;
        //public int RadioColumnCount;
        //public string ListValue;
        private bool nullable;
        //public bool dontShow;
        public bool readOnly;
        private bool isPrimaryKey;
        private bool isIdentity;
        private int length;
        private int scale;
        private byte precition;
        private int ordinalPosition;
        [NonSerialized]
        private TableInfo table;

        public TableInfo Table
        {
            get
            {
                return table;
            }
            internal set
            {
                table = value;
            }
        }

        public string ColumnName
        {
            get { return columnName; }
            set
            {
                if (columnName == value)
                    return;
                columnName = value;
            }
        }
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(title))
                    return columnName;
                return title;
            }
            set
            {
                if (title == value)
                    return;
                title = value;
            }
        }
        public DbType DataType
        {
            get { return dataType; }
            set
            {
                if (dataType == value)
                    return;
                dataType = value;
                if (dataType == DbType.Decimal || dataType == DbType.Double || dataType == DbType.Single || dataType == DbType.Currency)
                {
                    if (length == 0)
                        length = 18;
                    if (precition == 0)
                        precition = 2;
                }
            }
        }
        public bool Nullable
        {
            get { return nullable; }
            set
            {
                if (nullable == value)
                    return;
                nullable = value;
            }
        }
        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set
            {
                if (isPrimaryKey == value)
                    return;
                isPrimaryKey = value;
            }
        }
        public int Length
        {
            get { return length; }
            set
            {
                if (length == value)
                    return;
                length = value;
            }
        }
        public int Scale
        {
            get { return scale; }
            set
            {
                if (scale == value)
                    return;
                scale = value;
            }
        }
        public byte Precition
        {
            get { return precition; }
            set
            {
                if (precition == value)
                    return;
                precition = value;
            }
        }
        public int OrdinalPosition
        {
            get { return ordinalPosition; }
            set
            {
                if (ordinalPosition == value)
                    return;
                ordinalPosition = value;
            }
        }
        public bool IsIdentity
        {
            get { return isIdentity; }
            set
            {
                if (isIdentity == value)
                    return;
                isIdentity = value;
            }
        }
        private object tag;
        public object Tag
        {
            get { return tag; }
            set
            {
                if (tag == value)
                    return;
                tag = value;
            }
        }
        public object DefaultValue { get; set; }
        /// <summary>
        /// Used if DbType is Object. 
        /// geography is only supported.
        /// </summary>
        public string TypeName { get; set; }


        public string GetColumnDefinition(DatabaseType databaseType, bool isAlter=false)
        {
            string type = "";
            switch (databaseType)
            {
                case DatabaseType.SQLServer:
                    type += DbTypeUtility.GetSqlTypeNameFromDbType(dataType, TypeName);
                    break;
                case DatabaseType.Oracle:
                    type += DbTypeUtility.GetOracleTypeNameFromDbType(dataType);
                    break;
                case DatabaseType.OleDb:
                    type += DbTypeUtility.GetOleDbTypeNameFromDbType(dataType);
                    break;
            }
            switch (dataType)
            {
                case DbType.AnsiString:
                    if (length > 10000)
                        type = "varchar(MAX)";
                    else
                        goto case DbType.AnsiStringFixedLength;
                    break;
                case DbType.String:
                    if (length > 10000)
                        type = "nvarchar(MAX)";
                    else
                        goto case DbType.AnsiStringFixedLength;
                    break;
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                    type += String.Format("({0})", length);
                    break;
                case DbType.Decimal:
                    type += String.Format("({0},{1})", precition, scale);
                    break;
            }
            string def;
            if (databaseType == DatabaseType.SQLServer)
                def = String.Format("[{0}] {1}", columnName, type);
            else
                def = columnName + " " + type;
            //Nullable
            def += (this.nullable ? " NULL" : " NOT NULL");
            //Default value
            if (DefaultValue != null && !isAlter)
                def += " DEFAULT(" + DefaultValue.ToString() + ")";
            //Identity
            if (this.isIdentity)
                def += " IDENTITY";
            //Primary key
            if (this.isPrimaryKey)
                def += " PRIMARY KEY";
            return def;
        }

        internal static ColumnInfo FromSqlSchemaRow(DataRow row)
        {
            ColumnInfo ci = new ColumnInfo();
            ci.columnName = (string)row["column_name"];

            object o = row["ordinal_position"];
            if (o is short)
                ci.ordinalPosition = (short)o;
            else
                ci.ordinalPosition = (int)o;
            ci.dataType = DbTypeUtility.GetDbTypeFromSqlTypeName((string)row["data_type"]);
            string[] type = ((string)row["data_type"]).Split(' ');
            if (type.Length > 1 && type[1].ToLower() == "identity")
                ci.isIdentity = true;
            ci.nullable = ((string)row["is_nullable"] == "YES");
            //Length
            o = row["character_maximum_length"];
            if (o is DBNull)
                ci.length = 0;
            else if (o is short)
                ci.length = (short)o;
            else if (o is int)
                ci.length = (int)o;
            o = row["numeric_precision"];
            if (o is DBNull)
                ci.precition = 0;
            else
                ci.precition = (byte)o;
            o = row["numeric_scale"];
            if (o is DBNull)
                ci.scale = 0;
            else
                ci.scale = (int)o;
            return ci;
        }
        public static ColumnInfo FromSqlSpColumnsRow(DataRow row)
        {
            ColumnInfo ci = new ColumnInfo();

            ci.columnName = (string)row["COLUMN_NAME"];

            object o = row["ORDINAL_POSITION"];

            if (o is short)
                ci.ordinalPosition = (short)o;
            else
                ci.ordinalPosition = (int)o;
            ci.dataType = DbTypeUtility.GetDbTypeFromSqlTypeName((string)row["TYPE_NAME"]);
            string[] type = ((string)row["TYPE_NAME"]).Split(' ');
            if (type.Length > 1 && type[1].ToLower() == "identity")
                ci.isIdentity = true;
            ci.nullable = ((short)row["NULLABLE"] == 1);
            //Length
            o = row["CHAR_OCTET_LENGTH"];
            if (o is DBNull)
                ci.length = 0;
            else if (o is short)
                ci.length = (short)o;
            else if (o is int)
                ci.length = (int)o;
            if (ci.dataType == DbType.String || ci.dataType == DbType.StringFixedLength)
                ci.length /= 2;
            if (ci.dataType == DbType.Decimal || ci.dataType == DbType.Currency)
            {
                o = row["PRECISION"];
                if (o is DBNull)
                    ci.precition = 0;
                else
                    ci.precition = (byte)(int)o;
                o = row["SCALE"];
                if (o is DBNull)
                    ci.scale = 0;
                else
                    ci.scale = (short)o;
            }
            return ci;
        }
        internal static ColumnInfo FromOracleSchemaRow(DataRow row)
        {
            ColumnInfo ci = new ColumnInfo();

            ci.columnName = (string)row["COLUMN_NAME"];
            ci.ordinalPosition = (int)(decimal)row["ID"];
            ci.dataType = DbTypeUtility.GetDbTypeFromOracleTypeName((string)row["DATATYPE"]);
            ci.nullable = ((string)row["NULLABLE"] != "N");
            //Length
            object o = row["LENGTH"];
            if (o is DBNull)
                ci.length = 0;
            else if (o is decimal)
                ci.length = (int)(decimal)o;
            else if (o is int)
                ci.length = (int)o;
            if (((string)row["DATATYPE"]).ToLower() == "long")
                ci.length = int.MaxValue;
            o = row["PRECISION"];
            if (o is DBNull)
                ci.precition = 0;
            else if (o is decimal)
                ci.precition = (byte)(decimal)o;
            else if (o is byte)
                ci.precition = (byte)o;
            o = row["SCALE"];
            if (o is DBNull)
                ci.scale = 0;
            else if (o is decimal)
                ci.scale = (int)(decimal)o;
            else
                ci.scale = (int)o;
            return ci;
        }

        internal static ColumnInfo FromOleDbSchemaRow(DataRow row)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(ColumnInfo c1, ColumnInfo c2)
        {
            if ((object)c1 == null)
                if ((object)c2 == null)
                    return true;
                else
                    return false;
            else if ((object)c2 == null)
                return false;
            return c1.columnName == c2.columnName && c1.dataType == c2.dataType && c1.nullable == c2.nullable && c1.isPrimaryKey == c2.isPrimaryKey
                && ((c1.length > 10000 && c2.length > 10000) || (c1.length == c2.length))
                && c1.precition == c2.precition && c1.scale == c2.scale && c1.isIdentity == c2.isIdentity;
        }
        public static bool operator !=(ColumnInfo c1, ColumnInfo c2)
        {
            return !(c1 == c2);
        }

        public ForeignKeyRelation ForeignKeyRelation
        {
            get
            {
                foreach (var f in Table.ForeignKeys)
                {
                    if (f.FKField == columnName)
                        return f;
                }
                return null;
            }
        }


        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }

    [Serializable]
    public class ColumnInfoCollection : ObservableCollection<ColumnInfo>
    {
        public ColumnInfoCollection()
        {
            ItemInserted += new ListItemEventHandler<ColumnInfo>(ColumnInfoCollection_ItemInserted);
            BeforeItemRemove += new ListItemEventHandler<ColumnInfo>(ColumnInfoCollection_BeforeItemRemove);
        }

        void ColumnInfoCollection_BeforeItemRemove(object sender, ListItemEventArgs<ColumnInfo> e)
        {
            e.Item.Table = null;
        }

        void ColumnInfoCollection_ItemInserted(object sender, ListItemEventArgs<ColumnInfo> e)
        {
            e.Item.Table = table;
        }
        public ColumnInfo this[string columnName]
        {
            get
            {
                foreach (var c in this)
                {
                    if (c.ColumnName == columnName)
                        return c;
                }
                return null;
            }
        }
        [NonSerialized]
        private TableInfo table;
        public TableInfo Table
        {
            get
            {
                return table;
            }
            internal set
            {
                table = value;
                foreach (var c in this)
                {
                    c.Table = value;
                }
            }
        }
        public void Add(string name, DbType dataType)
        {
            ColumnInfo c = new ColumnInfo();
            c.ColumnName = name;
            c.DataType = dataType;
            Add(c);
        }
    }

}

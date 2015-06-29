using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace Unicorn.Data
{
    [Serializable]
    public class TableInfo
    {
        public TableInfo()
        {
            columns = new ColumnInfoCollection();
            columns.Table = this;
            ForeignKeys = new ForeignKeyRelationCollection(this);
            tableOwner = "dbo";
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            columns.Table = this;
        }
        private string tableName;
        private string tableOwner;
        private string tableType;
        private string title;
        private ColumnInfoCollection columns;
        public ForeignKeyRelationCollection ForeignKeys { get; private set; }

        public string TableName
        {
            get { return tableName; }
            set
            {
                if (tableName == value)
                    return;
                tableName = value;
            }
        }
        public string TableOwner
        {
            get { return tableOwner; }
            set
            {
                if (tableOwner == value)
                    return;
                tableOwner = value;
            }
        }
        public string TableType
        {
            get { return tableType; }
            set
            {
                if (tableType == value)
                    return;
                tableType = value;
            }
        }
        public string Title
        {
            get { return title; }
            set
            {
                if (title == value)
                    return;
                title = value;
            }
        }
        public ColumnInfoCollection Columns
        {
            get { return columns; }
        }
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(tableOwner))
                    return String.Format("\"{0}\"", tableName);
                return String.Format("\"{0}\".\"{1}\"", tableOwner, tableName);
            }
        }

        internal static TableInfo FromSqlSchemaRow(DataRow row)
        {
            TableInfo ti = new TableInfo();
            ti.tableName = (string)row["table_name"];
            ti.tableOwner = (string)row["table_schema"];
            ti.tableType = (string)row["table_type"];
            return ti;
        }
        internal static TableInfo FromSqlSPTables(object[] row)
        {
            TableInfo ti = new TableInfo();
            ti.tableName = (string)row[2];
            ti.tableOwner = (string)row[1];
            ti.tableType = (string)row[3];
            return ti;
        }

        internal static TableInfo FromOracleSchemaRow(DataRow row)
        {
            TableInfo ti = new TableInfo();
            ti.tableName = (string)row["TABLE_NAME"];
            ti.tableOwner = (string)row["OWNER"];
            ti.tableType = (string)row["TYPE"];
            return ti;
        }

        internal static TableInfo FromOleDbSchemaRow(DataRow row)
        {
            throw new NotImplementedException();
        }

        public void LoadDescriptions()
        {
            SchemaUtility.ReadDescriptions(this);
        }
        public ColumnInfo GetNameColumn()
        {
            if (Columns["Name"] != null)
                return Columns["Name"];
            else if (Columns["name"] != null)
                return Columns["name"];
            else
                for (int i = 0; i < columns.Count && i < 4; i++)
                {
                    if (!columns[i].IsPrimaryKey && columns[i].ColumnName.ToLower().EndsWith("name"))
                    {
                        return columns[i];
                    }
                }
            foreach (var c in Columns)
                if (!c.IsPrimaryKey)
                {
                    return c;
                }
            return Columns[0];
        }

        public string PrimaryKey
        {
            get
            {
                foreach (ColumnInfo c in columns)
                {
                    if (c.IsPrimaryKey)
                        return c.ColumnName;
                }
                return null;
            }
        }

    }
}

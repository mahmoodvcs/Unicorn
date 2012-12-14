using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Unicorn.Data
{
    public class DbTypeUtility
    {
        public static DbType GetDbTypeFromOracleTypeName(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "bfilet":
                    return DbType.Binary;
                case "blob":
                    return DbType.Binary;
                case "clob":
                case "nclob":
                    return DbType.String;
                case "number":
                    return DbType.VarNumeric;
                case "dec":
                case "decimal":
                    return DbType.Decimal;
                case "boolean":
                    return DbType.Boolean;
                case "int":
                case "integer":
                    return DbType.Int32;
                case "smallint":
                    return DbType.Int16;
                case "float":
                case "double":
                case "double precision":
                    return DbType.Double;
                case "char":
                    return DbType.AnsiStringFixedLength;
                case "nchar":
                    return DbType.StringFixedLength;
                case "varchar2":
                case "long":
                case "rawid":
                    return DbType.AnsiString;
                case "nvarchar2":
                case "urawid":
                    return DbType.String;
                case "raw":
                case "long raw":
                    return DbType.Binary;
                case "timestamp(6)":
                case "timestamp(5)":
                case "timestamp(4)":
                case "date":
                    return DbType.DateTime;
            }
            return DbType.Object;
        }
        public static string GetOracleTypeNameFromDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Binary:
                    return "blob";
                case DbType.VarNumeric:
                    return "number";
                case DbType.Decimal:
                    return "decimal";
                case DbType.Boolean:
                    return "boolean";
                case DbType.Int32:
                    return "integer";
                case DbType.Int16:
                    return "smallint";
                case DbType.Double:
                    return "double";
                case DbType.AnsiStringFixedLength:
                    return "char";
                case DbType.StringFixedLength:
                    return "nchar";
                case DbType.AnsiString:
                    return "varchar2";
                case DbType.String:
                    return "nvarchar2";
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return "date";
            }
            return "nvarchar2";
        }

     

        public static DbType GetDbTypeFromSqlTypeName(string typeName)
        {
            switch (typeName.Split(' ')[0].ToLower())
            {
                case "bigint":
                    return DbType.Int64;
                case "binary":
                    return DbType.Binary;
                case "bit":
                    return DbType.Boolean;
                case "char":
                    return DbType.AnsiStringFixedLength;
                case "date":
                case "datetime":
                    return DbType.DateTime;
                case "datetime2":
                    return DbType.DateTime2;
                case "datetimeoffset":
                    return DbType.DateTimeOffset;
                case "decimal":
                    return DbType.Decimal;
                case "filestream":
                    return DbType.Binary;
                case "float":
                    return DbType.Double;
                case "image":
                    return DbType.Binary;
                case "int":
                    return DbType.Int32;
                case "money":
                    return DbType.Decimal;
                case "nchar":
                    return DbType.StringFixedLength;
                case "ntext":
                    return DbType.String;
                case "nvarchar":
                    return DbType.String;
                case "real":
                    return DbType.Single;
                case "rowversion":
                    return DbType.Binary;
                case "smalldatetime":
                    return DbType.DateTime;
                case "smallint":
                    return DbType.Int16;
                case "smallmoney":
                    return DbType.Decimal;
                case "text":
                    return DbType.AnsiString;
                case "time":
                    return DbType.DateTime;
                case "timestamp":
                    return DbType.Binary;
                case "tinyint":
                    return DbType.Byte;
                case "uniqueidentifier":
                    return DbType.Guid;
                case "varBinary":
                    return DbType.Binary;
                case "varchar":
                    return DbType.AnsiString;
                case "variant":
                    return DbType.Object;
                default:
                    return DbType.Object;
            }
        }
        public static string GetSqlTypeNameFromDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Int64:
                case DbType.UInt64:
                    return "bigint";
                case DbType.Binary:
                    return "binary";
                case DbType.Boolean:
                    return "bit";
                case DbType.AnsiStringFixedLength:
                    return "char";
                case DbType.DateTime:
                case DbType.Date:
                case DbType.Time:
                    return "datetime";
                case DbType.DateTime2:
                    return "datetime2";
                case DbType.DateTimeOffset:
                    return "datetimeoffset";
                case DbType.Decimal:
                case DbType.VarNumeric:
                case DbType.Currency:
                    return "decimal";
                case DbType.Double:
                    return "float";
                case DbType.Int32:
                case DbType.UInt32:
                    return "int";
                case DbType.StringFixedLength:
                    return "nchar";
                case DbType.String:
                    return "nvarchar";
                case DbType.Single:
                    return "real";
                case DbType.Int16:
                case DbType.UInt16:
                    return "smallint";
                //case DbType.String:
                //    return "text";
                case DbType.Byte:
                    return "tinyint";
                case DbType.Guid:
                    return "uniqueidentifier";
                case DbType.AnsiString:
                    return "varchar";
                case DbType.Object:
                    return "variant";
                default:
                    return "nvarchar";
            }
        }

        internal static string GetOleDbTypeNameFromDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Int64:
                case DbType.UInt64:
                    return "long";
                case DbType.Binary:
                case DbType.Object:
                    return "binary";
                case DbType.Boolean:
                    return "bit";
                case DbType.AnsiStringFixedLength:
                case DbType.AnsiString:
                case DbType.StringFixedLength:
                case DbType.String:
                    return "text";
                case DbType.DateTime:
                case DbType.Date:
                case DbType.Time:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return "datetime";
                case DbType.Decimal:
                case DbType.VarNumeric:
                case DbType.Currency:
                    return "decimal";
                    break;
                case DbType.Double:
                    return "float";
                case DbType.Int32:
                case DbType.UInt32:
                    return "integer";
                case DbType.Single:
                    return "real";
                case DbType.Int16:
                case DbType.UInt16:
                    return "smallint";
                //case DbType.String:
                //    return "text";
                case DbType.Byte:
                    return "tinyint";
                case DbType.Guid:
                    return "uniqueidentifier";
                default:
                    return "text";
            }
        }

        public static bool IsStringType(DbType type)
        {
            switch (type)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return true;
            }
            return false;
        }
        public static bool IsNumericType(DbType type)
        {
            switch (type)
            {
                case DbType.Byte:
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                case DbType.Single:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.VarNumeric:
                    return true;
            }
            return false;
        }
        public static bool IsIntegerType(DbType type)
        {
            switch (type)
            {
                case DbType.Byte:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return true;
            }
            return false;
        }

        public static Type GetType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return typeof(string);
                case DbType.Boolean:
                    return typeof(bool);
                case DbType.Byte:
                    return typeof(byte);
                case DbType.Currency:
                case DbType.Decimal:
                    return typeof(decimal);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.Time:
                    return typeof(DateTime);
                case DbType.DateTimeOffset:
                    return typeof(TimeSpan);
                case DbType.Double:
                    return typeof(double);
                case DbType.Guid:
                    return typeof(Guid);
                case DbType.Int16:
                    return typeof(short);
                case DbType.Int32:
                    return typeof(int);
                case DbType.Int64:
                    return typeof(long);
                case DbType.SByte:
                    return typeof(sbyte);
                case DbType.Single:
                    return typeof(float);
                case DbType.UInt16:
                    return typeof(ushort);
                case DbType.UInt32:
                    return typeof(uint);
                case DbType.UInt64:
                    return typeof(ulong);
                case DbType.VarNumeric:
                    return typeof(decimal);
                default:
                    return typeof(string);
            }
        }
    }
}

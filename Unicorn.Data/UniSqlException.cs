using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Unicorn.Data
{
    public class UniSqlException : Exception
    {
        public UniSqlException(SqlException exception, string sqlCommand, params SqlParameter[] parameters)
        {
            SqlException = exception;
            SqlCommand = sqlCommand;
            Parameters = parameters;
        }
        public SqlException SqlException { get; private set; }
        public SqlParameter[] Parameters { get; private set; }
        public SqlErrorCollection Errors
        {
            get { return SqlException.Errors; }
        }

        public byte Class
        {
            get
            {
                return this.Errors[0].Class;
            }
        }
        public int LineNumber
        {
            get
            {
                return this.Errors[0].LineNumber;
            }
        }

        public int Number
        {
            get
            {
                return this.Errors[0].Number;
            }
        }

        public string Procedure
        {
            get
            {
                return this.Errors[0].Procedure;
            }
        }

        public string Server
        {
            get
            {
                return this.Errors[0].Server;
            }
        }

        public override string Source
        {
            get
            {
                return this.Errors[0].Source;
            }
        }

        public byte State
        {
            get
            {
                return this.Errors[0].State;
            }
        }

        public string SqlCommand
        {
            get;
            private set;
        }

    }
}

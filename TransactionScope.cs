using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Unicorn.Data
{
    public class TransactionScope : IDisposable
    {
        private DbTransaction _transaction = null;
        private bool committed = false;

        public TransactionScope()
        {
            _transaction = ConnectionManager.Connection.BeginTransaction();
        }
        public DbTransaction Transaction { get { return _transaction; } }

        public void Commit()
        {
            _transaction.Commit();
            committed = true;
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!committed)
                Rollback();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicorn.Data
{
    public class TransactionScope : IDisposable
    {
        bool isCompleted = false;
        public TransactionScope()
        {
            ConnectionManager.Instance.BeginTransaction();
        }

        public void Complete()
        {
            ConnectionManager.Instance.CommitTransaction();
            isCompleted = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!isCompleted)
            {
                ConnectionManager.Instance.RollbackTransaction();
            }
        }

        #endregion
    }
}

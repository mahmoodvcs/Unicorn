using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG.Framework.DAL
{
    public class SgTransactionScope : IDisposable
    {
        bool isCompleted = false;
        public SgTransactionScope()
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

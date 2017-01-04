﻿using DatabaseAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public class ApplicationDataFactory : IApplicationDataFactory
    {
        private static ApplicationData instance;

        public IApplicationData CreateApplicationData()
        {
            return CreateDbContext(false);
        }

        public ITransactionalApplicationData CreateTransactionalApplicationData(bool runTransaction)
        {
            return CreateDbContext(runTransaction);
        }


        public void Dispose()
        {
            if (instance != null)
                instance.Dispose();
        }

        private ITransactionalApplicationData CreateDbContext(bool beginTransaction)
        {
            if (instance == null || instance.IsDisposed)
                instance = new ApplicationData(beginTransaction);
            else if (beginTransaction && !instance.IsTransactionRunning)
                instance.BeginTransaction();
            else if (beginTransaction && instance.IsTransactionRunning)
            {
                if (instance.CommitUnfinishedTransaction)
                    instance.Commit();
                else
                    instance.Rollback();
                instance.BeginTransaction();
            }
            return instance;
        }
    }
}

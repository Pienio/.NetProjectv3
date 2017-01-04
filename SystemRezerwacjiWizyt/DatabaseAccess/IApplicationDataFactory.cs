﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public interface IApplicationDataFactory : IDisposable
    {
        IApplicationData CreateApplicationData();
        ITransactionalApplicationData CreateTransactionalApplicationData(bool runTransaction);
    }
}

//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace sophis.portfolio
{
    public class CSMTransactionEvent : IDisposable
    {
        public static void Register(string key, eMOrder e, CSMTransactionEvent prototype)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasBeenCreated(CSMTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasBeenDeleted(CSMTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasBeenModified(CSMTransaction original, CSMTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public virtual void UnRegister()
        {
            throw new NotImplementedException();
        }

        public enum eMOrder
        {
            M_oBefore = 0,
            M_oUpdate = 1,
            M_oAfter = 2
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

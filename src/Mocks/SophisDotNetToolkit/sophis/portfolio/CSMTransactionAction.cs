//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.tools;

namespace sophis.portfolio
{
    public class CSMTransactionAction : IDisposable
    {
        public static void Register(string key, eMOrder e, CSMTransactionAction prototype)
        {
        }

        public virtual void NotifyCreated(CSMTransaction transaction, CSMEventVector message, int event_id)
        {

        }

        public virtual void NotifyDeleted(CSMTransaction transaction, CSMEventVector message, int event_id)
        {

        }

        public virtual void NotifyModified(CSMTransaction original, CSMTransaction transaction, CSMEventVector message, int event_id)
        {

        }

        public virtual void UnRegister()
        {

        }

        public virtual void VoteForCreation(CSMTransaction transaction, int event_id)
        {

        }

        public virtual void VoteForDeletion(CSMTransaction transaction, int event_id)
        {

        }

        public virtual void VoteForModification(CSMTransaction original, CSMTransaction transaction, int event_id)
        {

        }

        public enum eMOrder
        {
            M_oBeforeDatabaseSaving = 0,
            M_oSavingInDataBase = 1,
            M_oBeforeSophisValidation = 2,
            M_oFOSophisValidation = 3,
            M_oBetweenSophisValidation = 4,
            M_oBOSophisValidation = 5,
            M_oAfterSophisValidation = 6,
            M_oLAST = 7
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

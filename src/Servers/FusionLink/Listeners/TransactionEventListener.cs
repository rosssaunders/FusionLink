//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using sophis.portfolio;

namespace RxdSolutions.FusionLink.Listeners
{
    public class TransactionEventListener : CSMTransactionEvent
    {
        public static event EventHandler<TransactionChangedEventArgs> TransactionChanged;

        public override bool HasBeenCreated(CSMTransaction transaction)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), false));

            return base.HasBeenCreated(transaction);
        }

        public override bool HasBeenDeleted(CSMTransaction transaction)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), false));

            return base.HasBeenDeleted(transaction);
        }

        public override bool HasBeenModified(CSMTransaction original, CSMTransaction transaction)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), false));

            return base.HasBeenModified(original, transaction);
        }
    }
}
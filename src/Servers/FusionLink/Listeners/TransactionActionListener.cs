//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.portfolio;
using sophis.tools;

namespace RxdSolutions.FusionLink.Listeners
{
    public class TransactionActionListener : CSMTransactionAction
    {
        public static event EventHandler<TransactionChangedEventArgs> TransactionChanged;

        public override void NotifyCreated(CSMTransaction transaction, CSMEventVector message, int event_id)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), true));

            base.NotifyCreated(transaction, message, event_id);
        }

        public override void NotifyDeleted(CSMTransaction transaction, CSMEventVector message, int event_id)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), true));

            base.NotifyDeleted(transaction, message, event_id);
        }

        public override void NotifyModified(CSMTransaction original, CSMTransaction transaction, CSMEventVector message, int event_id)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), true));

            base.NotifyModified(original, transaction, message, event_id);
        }
    }
}
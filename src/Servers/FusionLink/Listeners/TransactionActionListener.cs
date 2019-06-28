//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.portfolio;
using sophis.tools;

namespace RxdSolutions.FusionLink
{
    public class TransactionActionListener : CSMTransactionAction
    {
        public static event EventHandler<TransactionChangedEventArgs> TransactionChanged;

        public override void NotifyCreated(CSMTransaction transaction, CSMEventVector message, int event_id)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), false));

            base.NotifyCreated(transaction, message, event_id);
        }

        public override void NotifyDeleted(CSMTransaction transaction, CSMEventVector message, int event_id)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), false));

            base.NotifyDeleted(transaction, message, event_id);
        }

        public override void NotifyModified(CSMTransaction original, CSMTransaction transaction, CSMEventVector message, int event_id)
        {
            TransactionChanged?.Invoke(this, new TransactionChangedEventArgs(transaction.GetTransactionCode(), transaction.GetPositionID(), false));

            base.NotifyModified(original, transaction, message, event_id);
        }
    }
}
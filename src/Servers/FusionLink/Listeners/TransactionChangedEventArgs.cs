//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

namespace RxdSolutions.FusionLink
{
    public class TransactionChangedEventArgs
    {
        public int PositionId { get; }

        public long TransactionId { get; }

        public bool IsLocal { get; }

        public TransactionChangedEventArgs(long tranactionId, int positionId, bool isLocal)
        {
            TransactionId = tranactionId;
            PositionId = positionId;
            IsLocal = isLocal;
        }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.


namespace RxdSolutions.FusionLink.Listeners
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
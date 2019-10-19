//  Copyright (c) RXD Solutions. All rights reserved.


namespace RxdSolutions.FusionLink.Listeners
{
    public class PositionChangedEventArgs
    {
        public int PositionId { get; }

        public bool IsLocal { get; }

        public PositionChangedEventArgs(int positionId, bool isLocal)
        {
            PositionId = positionId;
            IsLocal = isLocal;
        }
    }
}
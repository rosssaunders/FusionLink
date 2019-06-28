//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

namespace RxdSolutions.FusionLink
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
//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class PositionValueReceivedEventArgs : ValueSentEventArgs
    {
        public int PositionId { get; private set; }

        public PositionValueReceivedEventArgs(int positionId, string column, object value)
             : base(column, value)

        {
            this.PositionId = positionId;
        }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using RxdSolutions.Sophis2Excel.Interface;

namespace RTD.Excel
{
    public class PositionValueReceivedEventArgs : ValueSentEventArgs
    {
        public int PositionId { get; private set; }

        public PositionValueReceivedEventArgs(int positionId, string column, DataTypeEnum dataType, object value)
             : base(column, dataType, value)

        {
            this.PositionId = positionId;
        }
    }
}
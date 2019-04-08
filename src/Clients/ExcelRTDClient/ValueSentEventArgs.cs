//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.Sophis2Excel.Interface;

namespace RTD.Excel
{
    public abstract class ValueSentEventArgs : EventArgs
    {
        public string Column { get; private set; }

        public DataTypeEnum DataType { get; private set; }

        public object Value { get; private set; }

        public ValueSentEventArgs(string column, DataTypeEnum dataType, object value)
        {
            this.Column = column;
            this.Value = value;
            this.DataType = dataType;
        }
    }
}
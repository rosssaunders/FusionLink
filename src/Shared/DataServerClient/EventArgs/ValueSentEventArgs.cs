//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.Client
{
    public abstract class ValueSentEventArgs : EventArgs
    {
        public string Column { get; private set; }

        public object Value { get; private set; }

        public ValueSentEventArgs(string column, object value)
        {
            this.Column = column;
            this.Value = value;
        }
    }
}
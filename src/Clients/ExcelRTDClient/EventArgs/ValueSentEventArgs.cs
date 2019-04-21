//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.RTDClient
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
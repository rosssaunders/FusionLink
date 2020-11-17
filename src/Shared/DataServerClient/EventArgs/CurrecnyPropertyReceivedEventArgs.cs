//  Copyright (c) RXD Solutions. All rights reserved.
using System;

namespace RxdSolutions.FusionLink.Client
{
    public class CurrencyPropertyReceivedEventArgs : EventArgs
    {
        public object Currency { get; private set; }

        public string Property { get; private set; }

        public object Value { get; private set; }

        public CurrencyPropertyReceivedEventArgs(object currency, string property, object value)
        {
            this.Currency = currency;
            this.Property = property;
            this.Value = value;
        }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.
using System;

namespace RxdSolutions.FusionLink.Client
{
    public class InstrumentPropertyReceivedEventArgs : EventArgs
    {
        public object Instrument { get; private set; }

        public string Property { get; private set; }

        public object Value { get; private set; }

        public InstrumentPropertyReceivedEventArgs(object instrument, string property, object value)
        {
            this.Instrument = instrument;
            this.Property = property;
            this.Value = value;
        }
    }
}
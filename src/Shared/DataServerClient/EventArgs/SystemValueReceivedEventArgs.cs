//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.Client
{
    public class SystemValueReceivedEventArgs : EventArgs
    {
        public SystemValueReceivedEventArgs(SystemProperty property, object value)
        {
            Property = property;
            Value = value;
        }

        public SystemProperty Property { get; private set; }

        public object Value { get; private set; }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.RTDClient
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
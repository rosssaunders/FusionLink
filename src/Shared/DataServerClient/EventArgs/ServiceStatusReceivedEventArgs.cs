//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.Client
{
    public class ServiceStatusReceivedEventArgs : EventArgs
    {
        public ServiceStatusReceivedEventArgs(ServiceStatus status)
        {
            Status = status;
        }

        public ServiceStatus Status { get; private set; }
    }
}
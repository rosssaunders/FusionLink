//  Copyright (c) RXD Solutions. All rights reserved.


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
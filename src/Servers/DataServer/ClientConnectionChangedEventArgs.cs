//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public class ClientConnectionChangedEventArgs : EventArgs
    {
        public ClientConnectionChangedEventArgs(ClientConnectionStatus status, IRealTimeCallbackClient client)
        {
            Status = status;
            Client = client;
        }

        public ClientConnectionStatus Status { get; private set; }

        public IRealTimeCallbackClient Client { get; private set; }
    }
}
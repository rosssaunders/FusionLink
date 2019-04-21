//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public class ClientConnectionChangedEventArgs : EventArgs
    {
        public ClientConnectionChangedEventArgs(ClientConnectionStatus status, IDataServiceClient client)
        {
            Status = status;
            Client = client;
        }

        public ClientConnectionStatus Status { get; private set; }

        public IDataServiceClient Client { get; private set; }
    }
}
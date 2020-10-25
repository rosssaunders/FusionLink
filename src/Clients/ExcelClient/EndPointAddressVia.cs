//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.ServiceModel;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public enum ConnectionType
    {
        Automatic,
        Manual
    }

    public class EndpointAddressVia
    {
        public EndpointAddressVia(EndpointAddress endpointAddress, Uri via, ConnectionType connectionType)
        {
            EndpointAddress = endpointAddress;
            Via = via;
            ConnectionType = connectionType;
        }

        public EndpointAddress EndpointAddress { get; }

        public Uri Via { get; }

        public ConnectionType ConnectionType { get; }
    }
}

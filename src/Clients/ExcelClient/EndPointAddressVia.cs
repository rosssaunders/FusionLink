using System;
using System.ServiceModel;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class EndPointAddressVia
    {
        public EndPointAddressVia(EndpointAddress endpointAddress, Uri via)
        {
            EndpointAddress = endpointAddress;
            Via = via;
        }

        public EndpointAddress EndpointAddress { get; }

        public Uri Via { get; }
    }
}

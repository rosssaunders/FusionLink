//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public class DataServerHostFactory
    {
        private static readonly string ServiceName = $"/";

        private static Uri GetListeningAddress()
        {
            return new Uri($"net.pipe://localhost/SophisDataService_{System.Diagnostics.Process.GetCurrentProcess().Id}");
        }

        public static ServiceHost Create(DataServer server)
        {
            var baseAddress = GetListeningAddress();
            var host = new ServiceHost(server, baseAddress);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport) {
                MaxReceivedMessageSize = int.MaxValue,
            };

            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            host.AddServiceEndpoint(typeof(IDataServiceServer), binding, ServiceName);

            // Add ServiceDiscoveryBehavior  
            var discoveryBehavior = new ServiceDiscoveryBehavior();
            discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
            host.Description.Behaviors.Add(discoveryBehavior);

            // Add a UdpDiscoveryEndpoint  
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            host.Open();

            return host;
        }
    }
}

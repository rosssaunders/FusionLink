//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Net.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public class DataServerHostFactory
    {
        private static readonly string ServiceName = $"/";

        public static Uri GetListeningAddress()
        {
            var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var sessionId = System.Diagnostics.Process.GetCurrentProcess().SessionId;

            return new Uri($"net.pipe://{Environment.MachineName}/FusionLink/{sessionId}/{processId}");
        }

        public static ServiceHost Create(DataServer server)
        {
            var baseAddress = GetListeningAddress();
            var host = new ServiceHost(server, baseAddress);

            //Add the NamedPipes endPoint
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue,
            };

            binding.MaxConnections = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            host.AddServiceEndpoint(typeof(IDataServiceServer), binding, ServiceName);

            // Add ServiceDiscoveryBehavior  
            var discoveryBehavior = new ServiceDiscoveryBehavior();
            var serviceAnnouncementEndpoint = new UdpAnnouncementEndpoint();
            discoveryBehavior.AnnouncementEndpoints.Add(serviceAnnouncementEndpoint);
            host.Description.Behaviors.Add(discoveryBehavior);

            // Add a UdpDiscoveryEndpoint  
            var serviceDiscoveryEndPoint = new UdpDiscoveryEndpoint();
            host.AddServiceEndpoint(serviceDiscoveryEndPoint);

            //Secure to only the current user
            host.Authorization.ServiceAuthorizationManager = new CurrentUserOnlyAuthorizationManager(serviceDiscoveryEndPoint);

            host.Open();

            return host;
        }
    }
}

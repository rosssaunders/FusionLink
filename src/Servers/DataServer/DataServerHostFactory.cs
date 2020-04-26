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
    public static class DataServerHostFactory
    {
        public static Uri GetBaseAddress()
        {
            int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            int sessionId = System.Diagnostics.Process.GetCurrentProcess().SessionId;

            return new Uri($"net.pipe://{Environment.MachineName}/FusionLink/{sessionId}/{processId}");
        }

        public static ServiceHost Create(RealTimeDataServer server)
        {
            var baseAddress = GetBaseAddress();
            var host = new ServiceHost(server, baseAddress);

            var binding = CreateBinding();

            host.AddServiceEndpoint(typeof(IRealTimeServer), binding, $"/");

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

        public static ServiceHost Create(OnDemandDataServer server)
        {
            var baseAddress = GetBaseAddress();
            var host = new ServiceHost(server, baseAddress);

            //Add the NamedPipes endPoint
            var binding = CreateBinding();

            host.AddServiceEndpoint(typeof(IOnDemandServer), binding, "/OnDemand");

            //Secure to only the current user
            host.Authorization.ServiceAuthorizationManager = new CurrentUserOnlyAuthorizationManager();

            host.Open();

            return host;
        }

        private static NetNamedPipeBinding CreateBinding()
        {
            //Add the NamedPipes endPoint
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue,
            };

            binding.MaxConnections = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            binding.SendTimeout = new TimeSpan(0, 5, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            return binding;
        }
    }
}

//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Net;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public static class DataServerHostFactory
    {
        private static Uri GetBaseAddress()
        {
            int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            int sessionId = System.Diagnostics.Process.GetCurrentProcess().SessionId;

            return new Uri($"net.tcp://{Dns.GetHostEntry("").HostName}:0/FusionLink/{sessionId}/{processId}/{Environment.UserName}");
        }

        private static Binding GetBinding()
        {
            return CreateTcpBinding();
        }

        public static ServiceHost Create(DataServers servers)
        {
            var baseAddress = GetBaseAddress();
            var host = new ServiceHost(servers, baseAddress);

            var binding = GetBinding();

            var realTimeEndPoint = host.AddServiceEndpoint(typeof(IRealTimeServer), binding, $"/");
            realTimeEndPoint.ListenUriMode = ListenUriMode.Unique;

            var onDemandEndPoint = host.AddServiceEndpoint(typeof(IOnDemandServer), binding, $"/");
            onDemandEndPoint.ListenUriMode = ListenUriMode.Unique;

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

        //public static ServiceHost Create(OnDemandDataServer server)
        //{
        //    var baseAddress = GetBaseAddress();
        //    var host = new ServiceHost(server, baseAddress);

        //    //Add the NamedPipes endPoint
        //    var binding = GetBinding();

        //    var endPoint = host.AddServiceEndpoint(typeof(IOnDemandServer), binding, "/OnDemand");
        //    endPoint.ListenUriMode = ListenUriMode.Unique;

        //    //Secure to only the current user
        //    host.Authorization.ServiceAuthorizationManager = new CurrentUserOnlyAuthorizationManager();

        //    host.Open();

        //    return host;
        //}

        private static NetTcpBinding CreateTcpBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue,
            };

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            binding.MaxConnections = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.SendTimeout = new TimeSpan(0, 5, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            return binding;
        }
    }
}

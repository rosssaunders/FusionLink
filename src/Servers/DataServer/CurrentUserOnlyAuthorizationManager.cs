﻿//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace RxdSolutions.FusionLink
{
    public class CurrentUserOnlyAuthorizationManager : ServiceAuthorizationManager
    {
        private readonly UdpDiscoveryEndpoint _udpDiscovery;
 
        public CurrentUserOnlyAuthorizationManager()
        {
        }

        public CurrentUserOnlyAuthorizationManager(UdpDiscoveryEndpoint udpDiscovery)
        {
            _udpDiscovery = udpDiscovery;
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            var currentUser = WindowsIdentity.GetCurrent()?.User;
            var contextUser = operationContext?.ServiceSecurityContext?.WindowsIdentity?.User;
            if ((currentUser == null || contextUser == null) && _udpDiscovery != null)
            {
                //Allow service discovery through
                if(operationContext.EndpointDispatcher.EndpointAddress.Uri.Equals(_udpDiscovery.Address.Uri))
                {
                    return true;
                }

                return false;
            }
            
            return currentUser.Equals(contextUser);
        }
    }
}

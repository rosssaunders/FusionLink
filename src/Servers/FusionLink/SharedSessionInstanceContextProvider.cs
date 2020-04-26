//  Copyright (c) RXD Solutions. All rights reserved.
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using RxdSolutions.FusionLink.Provider;

namespace RxdSolutions.FusionLink
{
    public class SharedSessionInstanceContextProvider : IInstanceContextProvider
    {
        private readonly FusionRealTimeProvider _provider;

        public SharedSessionInstanceContextProvider(FusionRealTimeProvider provider)
        {
            _provider = provider;
        }

        public InstanceContext GetExistingInstanceContext(Message message, IContextChannel channel)
        {
            return new InstanceContext(_provider);
        }

        public void InitializeInstanceContext(InstanceContext instanceContext, Message message, IContextChannel channel)
        {
        }

        public bool IsIdle(InstanceContext instanceContext)
        {
            return false;
        }

        public void NotifyIdle(InstanceContextIdleCallback callback, InstanceContext instanceContext)
        {
        }
    }
}

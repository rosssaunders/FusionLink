//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class DataServiceClientCallback : IDataServiceClient
    {
        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;
        public event EventHandler<PortfolioPropertyReceivedEventArgs> OnPortfolioPropertyReceived;
        public event EventHandler<SystemValueReceivedEventArgs> OnSystemValueReceived;
        public event EventHandler<ServiceStatusReceivedEventArgs> OnServiceStatusReceived;

        public void Heartbeat()
        {
            //DoNothing
        }

        public void SendPortfolioProperty(int portfolioId, PortfolioProperty property, object value)
        {
            OnPortfolioPropertyReceived?.Invoke(this, new PortfolioPropertyReceivedEventArgs(portfolioId, property, value));
        }

        public void SendPortfolioValue(int portfolioId, string column, object value)
        {
            OnPortfolioValueReceived?.Invoke(this, new PortfolioValueReceivedEventArgs(portfolioId, column, value));
        }

        public void SendPositionValue(int positionId, string column, object value)
        {
            OnPositionValueReceived?.Invoke(this, new PositionValueReceivedEventArgs(positionId, column, value));
        }

        public void SendServiceStaus(ServiceStatus status)
        {
            OnServiceStatusReceived?.Invoke(this, new ServiceStatusReceivedEventArgs(status));
        }

        public void SendSystemValue(SystemProperty property, object value)
        {
            OnSystemValueReceived?.Invoke(this, new SystemValueReceivedEventArgs(property, value));
        }
    }
}
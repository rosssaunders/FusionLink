﻿//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.Client
{
    public class DataServiceClientCallback : IDataServiceClient
    {
        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;
        public event EventHandler<PortfolioPropertyReceivedEventArgs> OnPortfolioPropertyReceived;
        public event EventHandler<SystemValueReceivedEventArgs> OnSystemValueReceived;
        public event EventHandler<ServiceStatusReceivedEventArgs> OnServiceStatusReceived;

        public DateTime LastReceivedMessageTimestamp { get; private set; }

        public void Heartbeat()
        {
            //DoNothing
        }

        public void SendPortfolioProperty(int portfolioId, PortfolioProperty property, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnPortfolioPropertyReceived?.Invoke(this, new PortfolioPropertyReceivedEventArgs(portfolioId, property, value));
        }

        public void SendPortfolioValue(int portfolioId, string column, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnPortfolioValueReceived?.Invoke(this, new PortfolioValueReceivedEventArgs(portfolioId, column, value));
        }

        public void SendPositionValue(int positionId, string column, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnPositionValueReceived?.Invoke(this, new PositionValueReceivedEventArgs(positionId, column, value));
        }

        public void SendServiceStaus(ServiceStatus status)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnServiceStatusReceived?.Invoke(this, new ServiceStatusReceivedEventArgs(status));
        }

        public void SendSystemValue(SystemProperty property, object value)
        {
            LastReceivedMessageTimestamp = DateTime.UtcNow;

            OnSystemValueReceived?.Invoke(this, new SystemValueReceivedEventArgs(property, value));
        }
    }
}
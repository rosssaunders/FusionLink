//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class DataServiceClientCallback : IDataServiceClient
    {
        public event EventHandler<PositionValueReceivedEventArgs> OnPositionValueReceived;
        public event EventHandler<PortfolioValueReceivedEventArgs> OnPortfolioValueReceived;
        public event EventHandler<PortfolioDateReceivedEventArgs> OnPortfolioDateReceived;

        public void SendPortfolioValue(int portfolioId, string column, object value)
        {
            OnPortfolioValueReceived?.Invoke(this, new PortfolioValueReceivedEventArgs(portfolioId, column, value));
        }

        public void SendPositionValue(int positionId, string column, object value)
        {
            OnPositionValueReceived?.Invoke(this, new PositionValueReceivedEventArgs(positionId, column, value));
        }

        public void SendPortfolioDate(DateTime portfolioDate)
        {
            OnPortfolioDateReceived?.Invoke(this, new PortfolioDateReceivedEventArgs(portfolioDate));
        }
    }
}
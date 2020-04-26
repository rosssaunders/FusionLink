//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public interface IRealTimeProvider
    {
        event EventHandler<DataAvailableEventArgs> DataAvailable;

        void RequestCalculate();

        void LoadPositions();

        void Start();

        void Stop();

        bool IsRunning { get; }

        void SubscribeToPortfolio(int portfolioId, string column);

        void SubscribeToPosition(int positionId, string column);

        void SubscribeToSystemValue(SystemProperty property);

        void SubscribeToPortfolioProperty(int id, PortfolioProperty property);

        void SubscribeToInstrumentProperty(object id, string property);
        
        void UnsubscribeFromPortfolio(int portfolioId, string column);

        void UnsubscribeFromPosition(int positionId, string column);

        void UnsubscribeFromSystemValue(SystemProperty property);

        void UnsubscribeFromPortfolioProperty(int id, PortfolioProperty property);

        void UnsubscribeFromInstrumentProperty(object id, string property);
    }
}

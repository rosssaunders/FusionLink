//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public interface IDataServerProvider
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

        void UnsubscribeToPortfolio(int portfolioId, string column);

        void UnsubscribeToPosition(int positionId, string column);

        void UnsubscribeToSystemValue(SystemProperty property);

        void UnsubscribeToPortfolioProperty(int id, PortfolioProperty property);

        List<int> GetPositions(int folioId, PositionsToRequest positions);

        List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate);

        List<PriceHistory> GetPriceHistory(string reference, DateTime startDate, DateTime endDate);

        List<CurvePoint> GetCurvePoints(string currency, string family, string reference);
    }
}

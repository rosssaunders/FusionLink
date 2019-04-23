//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class PortfolioValueExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public PortfolioValueExcelObservable(int portfolioId, string column, DataServiceClient rtdClient)
        {
            PortfolioId = portfolioId;
            Column = column;
            _rtdClient = rtdClient;
        }

        public int PortfolioId { get; }

        public string Column { get; }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.SubscribeToPortfolioValue(PortfolioId, Column);

            _rtdClient.OnPortfolioValueReceived += OnPortfolioValueSent;

            return new ActionDisposable(CleanUp);
        }

        private void OnPortfolioValueSent(object sender, PortfolioValueReceivedEventArgs args)
        {
            if (args.PortfolioId == PortfolioId && args.Column == Column)
            {
                _observer.OnNext(args.Value);
            }
        }

        void CleanUp()
        {
            _rtdClient.UnsubscribeToPortfolioValue(PortfolioId, Column);

            _rtdClient.OnPortfolioValueReceived -= OnPortfolioValueSent;
        }
    }
}
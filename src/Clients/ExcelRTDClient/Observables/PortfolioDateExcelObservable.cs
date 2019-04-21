//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class PortfolioDateExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public PortfolioDateExcelObservable(DataServiceClient rtdClient)
        {
            _rtdClient = rtdClient;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.SubscribeToPortfolioDate();

            _rtdClient.OnPortfolioDateReceived += OnPortfolioDateReceived;

            return new ActionDisposable(CleanUp);
        }

        private void OnPortfolioDateReceived(object sender, PortfolioDateReceivedEventArgs args)
        {
            _observer.OnNext(args.PortfolioDate);
        }

        void CleanUp()
        {
            _rtdClient.OnPortfolioDateReceived -= OnPortfolioDateReceived;
        }
    }
}
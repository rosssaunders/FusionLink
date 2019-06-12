//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class PortfolioPropertyExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public PortfolioPropertyExcelObservable(int portfolioId, PortfolioProperty property, DataServiceClient rtdClient)
        {
            PortfolioId = portfolioId;
            Property = property;
            _rtdClient = rtdClient;
        }

        public int PortfolioId { get; }

        public PortfolioProperty Property { get; }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.SubscribeToPortfolioProperty(PortfolioId, Property);

            _rtdClient.OnPortfolioPropertyReceived += OnPortfolioPropertySent;

            _observer.OnNext(ExcelEmpty.Value);

            return new ActionDisposable(CleanUp);
        }

        private void OnPortfolioPropertySent(object sender, PortfolioPropertyReceivedEventArgs args)
        {
            if (args.PortfolioId == PortfolioId && args.Property == Property)
            {
                if (args.Value is null)
                    _observer.OnNext(ExcelEmpty.Value);
                else
                    _observer.OnNext(args.Value);
            }
        }

        void CleanUp()
        {
            _rtdClient.UnsubscribeToPortfolioProperty(PortfolioId, Property);

            _rtdClient.OnPortfolioPropertyReceived -= OnPortfolioPropertySent;
        }
    }
}
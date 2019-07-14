//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;
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
            _rtdClient.OnPortfolioPropertyReceived += OnPortfolioPropertySent;

            try
            {
                _observer.OnNext(Resources.SubscribingToData);
                _rtdClient.SubscribeToPortfolioProperty(PortfolioId, Property);
            }
            catch(Exception ex)
            {
                _observer.OnNext(ex.Message);
            }

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
            _rtdClient.OnPortfolioPropertyReceived -= OnPortfolioPropertySent;

            try
            {
                _rtdClient.UnsubscribeToPortfolioProperty(PortfolioId, Property);
            }
            catch(Exception)
            {
                //Sink... not much we can do here.
            }
        }
    }
}
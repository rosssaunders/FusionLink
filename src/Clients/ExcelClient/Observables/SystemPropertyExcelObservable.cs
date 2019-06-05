//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class SystemPropertyExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public SystemPropertyExcelObservable(SystemProperty systemProperty, DataServiceClient rtdClient)
        {
            SystemProperty = systemProperty;

            _rtdClient = rtdClient;
        }

        public SystemProperty SystemProperty { get; }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.SubscribeToSystemValue(SystemProperty);

            _rtdClient.OnSystemValueReceived += OnPortfolioDateReceived;

            _observer.OnNext(ExcelEmpty.Value);

            return new ActionDisposable(CleanUp);
        }

        private void OnPortfolioDateReceived(object sender, SystemValueReceivedEventArgs args)
        {
            if(args.Property == SystemProperty)
            {
                if (args.Value is null)
                    _observer.OnNext(ExcelEmpty.Value);
                else
                    _observer.OnNext(args.Value);
            }
        }

        void CleanUp()
        {
            _rtdClient.UnsubscribeToSystemValue(SystemProperty);

            _rtdClient.OnSystemValueReceived -= OnPortfolioDateReceived;
        }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;
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
            _rtdClient.OnSystemValueReceived += OnPortfolioDateReceived;

            try
            {
                _observer.OnNext(Resources.SubscribingToData);
                _rtdClient.SubscribeToSystemValue(SystemProperty);
            }
            catch(Exception ex)
            {
                _observer.OnNext(ex.Message);
            }

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
            _rtdClient.OnSystemValueReceived -= OnPortfolioDateReceived;

            try
            {
                _rtdClient.UnsubscribeToSystemValue(SystemProperty);
            }
            catch (Exception)
            {
                //Sink...
            }   
        }
    }
}
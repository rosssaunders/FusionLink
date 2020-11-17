//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class CurrencyPropertyExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public CurrencyPropertyExcelObservable(object currency, string property, DataServiceClient rtdClient)
        {
            Currency = currency;
            Property = property;
            _rtdClient = rtdClient;
        }

        public object Currency { get; }

        public string Property { get; }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;
            _rtdClient.OnCurrencyPropertyReceived += OnCurrencyPropertySent;

            try
            {
                if (_rtdClient.State == System.ServiceModel.CommunicationState.Opened)
                    _observer.OnNext(Resources.SubscribingToData);
                else
                    _observer.OnNext(Resources.NotConnectedMessage);

                _rtdClient.SubscribeToCurrencyProperty(Currency, Property);
            }
            catch(Exception ex)
            {
                _observer.OnNext(ex.Message);
            }

            return new ActionDisposable(CleanUp);
        }

        private void OnCurrencyPropertySent(object sender, CurrencyPropertyReceivedEventArgs args)
        {
            if (Currency.Equals(args.Currency) && args.Property == Property)
            {
                if (args.Value is null)
                    _observer.OnNext(ExcelEmpty.Value);
                else
                    _observer.OnNext(args.Value);
            }
        }

        void CleanUp()
        {
            _rtdClient.OnCurrencyPropertyReceived -= OnCurrencyPropertySent;

            try
            {
                _rtdClient.UnsubscribeToCurrencyProperty(Currency, Property);
            }
            catch(Exception)
            {
                //Sink... not much we can do here.
            }
        }
    }
}
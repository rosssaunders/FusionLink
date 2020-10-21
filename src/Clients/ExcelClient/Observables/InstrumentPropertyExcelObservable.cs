//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class InstrumentPropertyExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public InstrumentPropertyExcelObservable(object instrument, string property, DataServiceClient rtdClient)
        {
            Instrument = instrument;
            Property = property;
            _rtdClient = rtdClient;
        }

        public object Instrument { get; }

        public string Property { get; }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;
            _rtdClient.OnInstrumentPropertyReceived += OnInstrumentPropertySent;

            try
            {
                if (_rtdClient.State == System.ServiceModel.CommunicationState.Opened)
                    _observer.OnNext(Resources.SubscribingToData);
                else
                    _observer.OnNext(Resources.NotConnectedMessage);

                _rtdClient.SubscribeToInstrumentProperty(Instrument, Property);
            }
            catch(Exception ex)
            {
                _observer.OnNext(ex.Message);
            }

            return new ActionDisposable(CleanUp);
        }

        private void OnInstrumentPropertySent(object sender, InstrumentPropertyReceivedEventArgs args)
        {
            if (Instrument.Equals(args.Instrument) && args.Property == Property)
            {
                if (args.Value is null)
                    _observer.OnNext(ExcelEmpty.Value);
                else
                    _observer.OnNext(args.Value);
            }
        }

        void CleanUp()
        {
            _rtdClient.OnInstrumentPropertyReceived -= OnInstrumentPropertySent;

            try
            {
                _rtdClient.UnsubscribeToInstrumentProperty(Instrument, Property);
            }
            catch(Exception)
            {
                //Sink... not much we can do here.
            }
        }
    }
}
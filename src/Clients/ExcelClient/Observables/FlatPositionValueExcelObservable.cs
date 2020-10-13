//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class FlatPositionValueExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public FlatPositionValueExcelObservable(int portfolioId, int instrumentId, string column, DataServiceClient rtdClient)
        {
            PortfolioId = portfolioId;
            InstrumentId = instrumentId;
            Column = column;
            _rtdClient = rtdClient;
        }

        public int PortfolioId { get; }
        
        public int InstrumentId { get; }

        public string Column { get; }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.OnFlatPositionValueReceived += OnDataReceived;

            try
            {
                _observer.OnNext(Resources.SubscribingToData);
                _rtdClient.SubscribeToFlatPositionValue(PortfolioId, InstrumentId, Column);
            }
            catch(Exception ex)
            {
                _observer.OnNext(ex.Message);
            }

            return new ActionDisposable(CleanUp);
        }

        private void OnDataReceived(object sender, FlatPositionValueReceivedEventArgs args)
        {
            if (args.PortfolioId == PortfolioId && args.InstrumentId == InstrumentId && args.Column == Column)
            {
                if (args.Value is null)
                    _observer.OnNext(ExcelEmpty.Value);
                else
                    _observer.OnNext(args.Value);
            }
        }

        void CleanUp()
        {
            _rtdClient.OnFlatPositionValueReceived -= OnDataReceived;

            try
            {
                _rtdClient.UnsubscribeToFlatPositionValue(PortfolioId, InstrumentId, Column);
            }
            catch(Exception)
            {
                //sink... not much we can do
            }
        }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class LastMessageReceivedTimeObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public LastMessageReceivedTimeObservable(DataServiceClient rtdClient)
        {
            _rtdClient = rtdClient;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.OnPositionValueReceived += OnMessageReceived;
            _rtdClient.OnPortfolioValueReceived += OnMessageReceived;
            _rtdClient.OnPortfolioPropertyReceived += OnMessageReceived;
            _rtdClient.OnInstrumentPropertyReceived += OnMessageReceived;
            _rtdClient.OnSystemValueReceived += OnMessageReceived;
            _rtdClient.OnServiceStatusReceived += OnMessageReceived;
            _rtdClient.OnFlatPositionValueReceived += OnMessageReceived;

            _observer.OnNext(_rtdClient.LastMessageReceivedTime.ToLocalTime());
    
            return new ActionDisposable(CleanUp);
        }

        private void OnMessageReceived(object sender, EventArgs e)
        {
            _observer.OnNext(_rtdClient.LastMessageReceivedTime.ToLocalTime());
        }

        private void CleanUp()
        {
            _rtdClient.OnPositionValueReceived -= OnMessageReceived;
            _rtdClient.OnPortfolioValueReceived -= OnMessageReceived;
            _rtdClient.OnPortfolioPropertyReceived -= OnMessageReceived;
            _rtdClient.OnSystemValueReceived -= OnMessageReceived;
            _rtdClient.OnServiceStatusReceived -= OnMessageReceived;
            _rtdClient.OnFlatPositionValueReceived -= OnMessageReceived;
            _rtdClient.OnInstrumentPropertyReceived -= OnMessageReceived;
        }
    }
}
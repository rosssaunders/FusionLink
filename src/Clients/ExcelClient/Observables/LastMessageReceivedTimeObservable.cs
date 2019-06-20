//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;

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
            _rtdClient.OnSystemValueReceived += OnMessageReceived;
            _rtdClient.OnServiceStatusReceived += OnMessageReceived;
        
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
        }
    }
}
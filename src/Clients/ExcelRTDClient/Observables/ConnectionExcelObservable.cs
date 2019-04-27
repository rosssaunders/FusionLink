//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ConnectionStringExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public ConnectionStringExcelObservable(DataServiceClient rtdClient)
        {
            _rtdClient = rtdClient;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.OnConnectionStatusChanged += OnConnectionStatusChanged;

            if(_rtdClient.Connection is object)
            {
                _observer.OnNext(_rtdClient.Connection.Uri.ToString());
            }
            else
            {
                _observer.OnNext("Not connected");
            }

            return new ActionDisposable(CleanUp);
        }

        private void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            _observer.OnNext(_rtdClient.Connection.Uri.ToString());
        }

        private void CleanUp()
        {
            _rtdClient.OnConnectionStatusChanged -= OnConnectionStatusChanged;
        }
    }
}
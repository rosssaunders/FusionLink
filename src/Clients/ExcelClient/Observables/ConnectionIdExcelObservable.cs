//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ConnectionIdExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public ConnectionIdExcelObservable(DataServiceClient rtdClient)
        {
            _rtdClient = rtdClient;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.OnConnectionStatusChanged += OnConnectionStatusChanged;

            if(_rtdClient.Connection is object)
            {
                _observer.OnNext(new ConnectionBuilder(_rtdClient.Connection.Uri).GetConnectionName());
            }
            else
            {
                _observer.OnNext(Resources.NotConnectedMessage);
            }

            return new ActionDisposable(CleanUp);
        }

        private void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if(_rtdClient.Connection is object)
            {
                _observer.OnNext(new ConnectionBuilder(_rtdClient.Connection.Uri).GetConnectionName());
            }
            else
            {
                _observer.OnNext(Resources.NotConnectedMessage);
            }
        }

        private void CleanUp()
        {
            _rtdClient.OnConnectionStatusChanged -= OnConnectionStatusChanged;
        }
    }
}
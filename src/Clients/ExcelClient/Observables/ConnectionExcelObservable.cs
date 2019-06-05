//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ConnectionNameExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private readonly ConnectionMonitor _connectionMonitor;
        private IExcelObserver _observer;

        public ConnectionNameExcelObservable(DataServiceClient rtdClient, ConnectionMonitor connectionMonitor)
        {
            _rtdClient = rtdClient;
            _connectionMonitor = connectionMonitor;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.OnConnectionStatusChanged += OnConnectionStatusChanged;
            _connectionMonitor.AvailableEndpointsChanged += AvailableEndpointsChanged;

            if (_rtdClient.Connection is object)
            {
                _observer.OnNext(ConnectionHelper.GetConnectionName(_rtdClient.Connection.Uri));
            }
            else
            {
                _observer.OnNext(Resources.NotConnectedMessage);
            }

            return new ActionDisposable(CleanUp);
        }

        private void AvailableEndpointsChanged(object sender, EventArgs e)
        {
            UpdateConnectionName();
        }

        private void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            UpdateConnectionName();
        }

        private void UpdateConnectionName()
        {
            if (_rtdClient.Connection is object)
            {
                _observer.OnNext(ConnectionHelper.GetConnectionName(_rtdClient.Connection.Uri));
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
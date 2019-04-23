//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Linq;
using ExcelDna.Integration;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class AvailableConnectionsStatusExcelObservable : IExcelObservable
    {
        private readonly ConnectionMonitor _monitor;
        private IExcelObserver _observer;

        public AvailableConnectionsStatusExcelObservable(ConnectionMonitor monitor)
        {
            _monitor = monitor;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _monitor.AvailableEndpointsChanged += OnAvailableEndpointsChanged;

            SendConnectionList();

            return new ActionDisposable(CleanUp);
        }

        private void OnAvailableEndpointsChanged(object sender, EventArgs e)
        {
            SendConnectionList();
        }

        private void SendConnectionList()
        {
            if (_monitor.AvailableEndpoints.Count == 0)
            {
                _observer.OnNext("No connections available");
            }
            else
            {
                var allConnections = string.Join(",", _monitor.AvailableEndpoints.Select(x => x.Uri.ToString()));
                _observer.OnNext(allConnections);
            }
        }

        private void CleanUp()
        {
            _monitor.AvailableEndpointsChanged -= OnAvailableEndpointsChanged;
        }
    }
}
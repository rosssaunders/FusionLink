//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Linq;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class AvailableConnectionsExcelObservable : IExcelObservable
    {
        private readonly ConnectionMonitor _monitor;
        private IExcelObserver _observer;

        public AvailableConnectionsExcelObservable(ConnectionMonitor monitor)
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
            if(_monitor.IsSearchingForEndPoints)
            {
                _observer.OnNext(Resources.SearchingForServersMessage);
            }
            else if (_monitor.AvailableEndpoints.Count == 0)
            {
                _observer.OnNext(Resources.NoEndPointsAvailableMessage);
            }
            else
            {
                string allConnections = string.Join(",", _monitor.AvailableEndpoints.Select(x => ConnectionHelper.GetConnectionId(x.Uri)));
                _observer.OnNext(allConnections);
            }
        }

        private void CleanUp()
        {
            _monitor.AvailableEndpointsChanged -= OnAvailableEndpointsChanged;
        }
    }
}
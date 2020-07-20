//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Linq;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class AvailableConnectionsExcelObservable : IExcelObservable
    {
        private readonly AvailableConnections _monitor;
        private IExcelObserver _observer;

        public AvailableConnectionsExcelObservable(AvailableConnections monitor)
        {
            _monitor = monitor;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _monitor.AvailableEndpointsChanged += OnAvailableEndpointsChanged;

            SendConnectionListToExcel();

            return new ActionDisposable(CleanUp);
        }

        private void OnAvailableEndpointsChanged(object sender, EventArgs e)
        {
            SendConnectionListToExcel();
        }

        private void SendConnectionListToExcel()
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
                string allConnections = string.Join(",", _monitor.AvailableEndpoints.Select(x => new ConnectionBuilder(x.Via).GetConnectionName()));
                _observer.OnNext(allConnections);
            }
        }

        private void CleanUp()
        {
            _monitor.AvailableEndpointsChanged -= OnAvailableEndpointsChanged;
        }
    }
}
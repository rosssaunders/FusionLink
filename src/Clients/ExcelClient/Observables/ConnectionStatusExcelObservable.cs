﻿//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ConnectionStatusExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public ConnectionStatusExcelObservable(DataServiceClient rtdClient)
        {
            _rtdClient = rtdClient;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.OnConnectionStatusChanged += OnConnectionStatusChanged;

            _observer.OnNext(_rtdClient.State.ToString());

            return new ActionDisposable(CleanUp);
        }

        private void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            _observer.OnNext(_rtdClient.State.ToString());
        }

        private void CleanUp()
        {
            _rtdClient.OnConnectionStatusChanged -= OnConnectionStatusChanged;
        }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ServiceStatusExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public ServiceStatusExcelObservable(DataServiceClient rtdClient)
        {
            _rtdClient = rtdClient;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.OnServiceStatusReceived += OnServiceStatusReceived;

            _observer.OnNext(_rtdClient.GetServiceStatus().ToString());

            return new ActionDisposable(CleanUp);
        }

        private void OnServiceStatusReceived(object sender, ServiceStatusReceivedEventArgs e)
        {
            _observer.OnNext(e.Status.ToString());
        }

        private void CleanUp()
        {
            _rtdClient.OnServiceStatusReceived -= OnServiceStatusReceived;
        }
    }
}
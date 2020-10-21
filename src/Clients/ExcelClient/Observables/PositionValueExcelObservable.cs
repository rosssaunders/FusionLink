//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using ExcelDna.Integration;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.ExcelClient.Properties;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class PositionValueExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public PositionValueExcelObservable(int positionId, string column, DataServiceClient rtdClient)
        {
            PositionId = positionId;
            Column = column;
            _rtdClient = rtdClient;
        }

        public int PositionId { get; }

        public string Column { get; }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.OnPositionValueReceived += OnDataReceived;

            try
            {
                if (_rtdClient.State == System.ServiceModel.CommunicationState.Opened)
                    _observer.OnNext(Resources.SubscribingToData);
                else
                    _observer.OnNext(Resources.NotConnectedMessage);

                _rtdClient.SubscribeToPositionValue(PositionId, Column);
            }
            catch(Exception ex)
            {
                _observer.OnNext(ex.Message);
            }

            return new ActionDisposable(CleanUp);
        }

        private void OnDataReceived(object sender, PositionValueReceivedEventArgs args)
        {
            if (args.PositionId == PositionId && args.Column == Column)
            {
                if (args.Value is null)
                    _observer.OnNext(ExcelEmpty.Value);
                else
                    _observer.OnNext(args.Value);
            }
        }

        void CleanUp()
        {
            _rtdClient.OnPositionValueReceived -= OnDataReceived;

            try
            {
                _rtdClient.UnsubscribeToPositionValue(PositionId, Column);
            }
            catch(Exception)
            {
                //sink... not much we can do
            }
        }
    }
}
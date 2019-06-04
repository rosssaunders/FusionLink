//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using ExcelDna.Integration;

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

            _rtdClient.SubscribeToPositionValue(PositionId, Column);

            _rtdClient.OnPositionValueReceived += OnDataReceived;

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
            _rtdClient.UnsubscribeToPositionValue(PositionId, Column);

            _rtdClient.OnPositionValueReceived -= OnDataReceived;
        }
    }
}
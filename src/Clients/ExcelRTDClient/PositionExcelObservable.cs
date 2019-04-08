//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using RTD.Excel;
using RxdSolutions.Sophis2Excel;
using RxdSolutions.Sophis2Excel.Interface;

namespace ExcelDna.Integration.RxExcel
{
    public class PositionExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public PositionExcelObservable(int positionId, string column, DataServiceClient rtdClient)
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

            _rtdClient.SubscribeToPosition(PositionId, Column);

            _rtdClient.OnPositionValueReceived += _rtdClient_OnPositionValueSent;

            return new ActionDisposable(CleanUp);
        }

        private void _rtdClient_OnPositionValueSent(object sender, PositionValueReceivedEventArgs args)
        {
            if (args.PositionId == PositionId && args.Column == Column)
            {
                _observer.OnNext(args.Value);
            }   
        }

        void CleanUp()
        {
            // Somehow clean up the link we made / the registration we set up
            _rtdClient.OnPositionValueReceived -= _rtdClient_OnPositionValueSent;
        }

        class ActionDisposable : IDisposable
        {
            Action _disposeAction;
            public ActionDisposable(Action disposeAction)
            {
                _disposeAction = disposeAction;
            }
            public void Dispose()
            {
                _disposeAction();
                Debug.WriteLine("Disposed");
            }
        }
    }
}
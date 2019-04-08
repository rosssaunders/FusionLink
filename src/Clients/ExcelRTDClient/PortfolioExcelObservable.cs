//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Diagnostics;
using RTD.Excel;
using RxdSolutions.Sophis2Excel.Interface;

namespace ExcelDna.Integration.RxExcel
{
    public class PortfolioExcelObservable : IExcelObservable
    {
        private readonly DataServiceClient _rtdClient;
        private IExcelObserver _observer;

        public PortfolioExcelObservable(int portfolioId, string column, DataServiceClient rtdClient)
        {
            PortfolioId = portfolioId;
            Column = column;
            _rtdClient = rtdClient;
        }

        public int PortfolioId { get; }

        public string Column { get; }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            _observer = observer;

            _rtdClient.SubscribeToPortfolio(PortfolioId, Column);

            _rtdClient.OnPortfolioValueReceived += _rtdClient_OnPortfolioValueSent;

            return new ActionDisposable(CleanUp);
        }

        private void _rtdClient_OnPortfolioValueSent(object sender, PortfolioValueReceivedEventArgs args)
        {
            if (args.PortfolioId == PortfolioId && args.Column == Column)
            {
                _observer.OnNext(args.Value);
            }
        }

        void CleanUp()
        {
            _rtdClient.OnPortfolioValueReceived -= _rtdClient_OnPortfolioValueSent;
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
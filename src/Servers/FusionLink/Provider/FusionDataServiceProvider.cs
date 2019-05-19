//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Threading;
using RxdSolutions.FusionLink.Interface;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    public class FusionDataServiceProvider : IDataServerProvider
    {
        private readonly SynchronizationContext _context;
        private readonly IGlobalFunctions _globalFunctions;

        public event EventHandler<DataAvailableEventArgs> DataAvailable;

        private readonly CellSubscriptions<PortfolioCellValue> _portfolioSubscriptions;
        private readonly CellSubscriptions<PositionCellValue> _positionSubscriptions;
        private readonly Dictionary<SystemProperty, SystemValue> _systemValueSubscriptions;

        public bool IsRunning { get; private set; }

        public FusionDataServiceProvider(SynchronizationContext context, IGlobalFunctions globalFunctions)
        {
            _context = context;
            _globalFunctions = globalFunctions;

            _portfolioSubscriptions = new CellSubscriptions<PortfolioCellValue>((i, s) => new PortfolioCellValue(i, s));
            _positionSubscriptions = new CellSubscriptions<PositionCellValue>((i, s) => new PositionCellValue(i, s));
            _systemValueSubscriptions = new Dictionary<SystemProperty, SystemValue>();

            _globalFunctions.PortfolioCalculationEnded += GlobalFunctions_PortfolioCalculationEnded;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public bool TryGetPositions(int folioId, PositionsToRequest positions, out List<int> results)
        {
            results = new List<int>();
            var innerResults = new List<int>();
            var result = true;

            _context.Send(state => {

                using (var portfolio = CSMPortfolio.GetCSRPortfolio(folioId))
                {
                    if (!portfolio.IsLoaded())
                    {
                        result = false;
                        return;
                    }

                    GetPositionsUI(folioId, positions, innerResults);
                }

            }, null);

            if(result)
                results.AddRange(innerResults);
            
            return result;
        }

        private void GetPositionsUI(int folioId, PositionsToRequest positions, List<int> results)
        {
            using (var portfolio = CSMPortfolio.GetCSRPortfolio(folioId))
            {
                int positionCount = portfolio.GetTreeViewPositionCount();
                for (int i = 0; i < positionCount; i++)
                {
                    using (var position = portfolio.GetNthTreeViewPosition(i))
                    {
                        switch (positions)
                        {
                            case PositionsToRequest.All:
                                results.Add(position.GetIdentifier());
                                break;

                            case PositionsToRequest.Open:
                                if (position.GetInstrumentCount() != 0)
                                {
                                    results.Add(position.GetIdentifier());
                                }
                                break;
                        }
                    }
                }

                int childPortfolioCount = portfolio.GetChildCount();

                for (int i = 0; i < childPortfolioCount; i++)
                {
                    using (var childPortfolio = portfolio.GetNthChild(i))
                    {
                        GetPositionsUI(childPortfolio.GetCode(), positions, results);
                    }
                }
            }
        }

        public void SubscribeToPortfolio(int portfolioId, string column)
        {
            _context.Post(state => {

                _portfolioSubscriptions.Add(portfolioId, column);

                var da = new DataAvailableEventArgs();
                da.PortfolioValues.Add((portfolioId, column), _portfolioSubscriptions.Get(portfolioId, column).GetValue());

                DataAvailable?.Invoke(this, da);

            }, null);

        }

        public void SubscribeToPosition(int positionId, string column)
        {
            _context.Post(state => {

                _positionSubscriptions.Add(positionId, column);

                var da = new DataAvailableEventArgs();
                da.PositionValues.Add((positionId, column), _positionSubscriptions.Get(positionId, column).GetValue());

                DataAvailable?.Invoke(this, da);

            }, null);

        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            _context.Post(state => {

                _systemValueSubscriptions.Add(property, GetSystemValueProperty(property));

                var da = new DataAvailableEventArgs();
                da.SystemValues.Add(property, _systemValueSubscriptions[property].GetValue());

                DataAvailable?.Invoke(this, da);

            }, null);

        }

        public void UnsubscribeToPortfolio(int portfolioId, string column)
        {
            _context.Post(state => {

                _portfolioSubscriptions.Remove(portfolioId, column);

            }, null);

        }

        public void UnsubscribeToPosition(int positionId, string column)
        {
            _context.Post(state => {

                _positionSubscriptions.Add(positionId, column);

            }, null);

        }

        public void UnsubscribeToSystemValue(SystemProperty property)
        {
            _context.Post(state => {

                _systemValueSubscriptions.Remove(property);

            }, null);
        }

        private void GlobalFunctions_PortfolioCalculationEnded(object sender, EventArgs e)
        {
            if(IsRunning)
            {
                //Sophis calls the global callback prior to performing their internal calculations so post the refresh to the back of the queue
                _context.Post(d => {

                    RefreshData();

                }, null);
            }
                
        }

        private void RefreshData()
        {
            var args = new DataAvailableEventArgs();

            void RefreshPortfolioCells()
            {
                foreach (var cell in _portfolioSubscriptions.GetCells())
                {
                    args.PortfolioValues.Add((cell.FolioId, cell.ColumnName), cell.GetValue());
                }
            }

            void RefreshPositionCells()
            {
                foreach (var cell in _positionSubscriptions.GetCells())
                {
                    args.PositionValues.Add((cell.PositionId, cell.ColumnName), cell.GetValue());
                }
            }

            void RefreshSystemValues()
            {
                foreach (var value in _systemValueSubscriptions)
                {
                    args.SystemValues.Add(value.Key, value.Value.GetValue());
                }
            }

            RefreshPortfolioCells();

            RefreshPositionCells();

            RefreshSystemValues();

            DataAvailable?.Invoke(this, args);
        }

        private SystemValue GetSystemValueProperty(SystemProperty property)
        {
            switch (property)
            {
                case SystemProperty.PortfolioDate:
                    return new PortfolioDateValue();
            }

            throw new InvalidOperationException();
        }
    }
}
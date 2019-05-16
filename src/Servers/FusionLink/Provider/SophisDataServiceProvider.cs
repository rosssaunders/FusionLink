//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using RxdSolutions.FusionLink.Interface;
using sophis.market_data;
using sophis.portfolio;
using Sophis.FolioView;
using Sophis.Portfolio;
using sophisTools;

namespace RxdSolutions.FusionLink
{
    public class SophisDataServiceProvider : IDataServerProvider
    {
        private readonly SynchronizationContext _context;
        private readonly GlobalFunctions _globalFunctions;
        private readonly Queue<int> _portfoliosToLoad = new Queue<int>();
        //private readonly Dictionary<string, CSMPortfolioColumn> _columnCache;

        private TimeSpan _lastRefreshTimer;

        private PortfolioRefreshEventsHandler _handler;

        public event EventHandler<DataAvailableEventArgs> DataAvailable;

        private readonly CellSubscriptions<PortfolioCellValue> _portfolioSubscriptions;
        private readonly CellSubscriptions<PositionCellValue> _positionSubscriptions;
        private readonly Dictionary<SystemProperty, SystemValue> _systemValueSubscriptions;

        public SophisDataServiceProvider(SynchronizationContext context, GlobalFunctions globalFunctions)
        {
            _context = context;
            _globalFunctions = globalFunctions;
            _lastRefreshTimer = default;
            //_columnCache = new Dictionary<string, CSMPortfolioColumn>();

            _handler = new PortfolioRefreshEventsHandler();
            _handler.DeferredHandling += PortfolioRefreshOnChanged;
            _handler.Register();

            _portfolioSubscriptions = new CellSubscriptions<PortfolioCellValue>((i, s) => new PortfolioCellValue(i, s));
            _positionSubscriptions = new CellSubscriptions<PositionCellValue>((i, s) => new PositionCellValue(i, s));


            //_globalFunctions.PortfolioAdditionEnded += GlobalFunctions_PortfolioAdditionEnded;
            //_globalFunctions.PortfolioCalculationEnded += GlobalFunctions_PortfolioCalculationEnded;
            //_globalFunctions.PortfolioRefreshVersionChanged += _globalFunctions_PortfolioRefreshVersionChanged;
        }

        private void _globalFunctions_PortfolioRefreshVersionChanged(object sender, EventArgs e)
        {
            var args = new DataAvailableEventArgs();

            foreach (var cell in _portfolioSubscriptions.GetCells())
            {
                args.PortfolioValues.Add((cell.FolioId, cell.ColumnName), cell.GetValue());
            }

            DataAvailable?.Invoke(this, args);
        }

        private void GlobalFunctions_PortfolioCalculationEnded(object sender, EventArgs e)
        {
            var args = new DataAvailableEventArgs();

            foreach (var cell in _portfolioSubscriptions.GetCells())
            {
                args.PortfolioValues.Add((cell.FolioId, cell.ColumnName), cell.GetValue());
            }

            DataAvailable?.Invoke(this, args);
        }

        private void GlobalFunctions_PortfolioAdditionEnded(object sender, EventArgs e)
        {
            var args = new DataAvailableEventArgs();

            foreach (var cell in _portfolioSubscriptions.GetCells())
            {
                args.PortfolioValues.Add((cell.FolioId, cell.ColumnName), cell.GetValue());
            }

            DataAvailable?.Invoke(this, args);
        }

        private void PortfolioRefreshOnChanged(PortfolioRefreshEventsHandler arg1, ModifiedEntities arg2)
        {
            var args = new DataAvailableEventArgs();

            void RefreshPortfolioCells()
            {
                var cellsToRefresh = new List<PortfolioCellValue>();

                if (arg2.RedrawAll || arg2.RefreshAllPortfolios || arg2.RedrawAllPortfolios)
                {
                    cellsToRefresh.AddRange(_portfolioSubscriptions.GetCells());
                }
                else
                {
                    var allPortfolios = new HashSet<int>();

                    foreach (var folio in arg2.PortfoliosToRedraw)
                    {
                        allPortfolios.Add(folio);
                    }

                    foreach (var folio in arg2.PortfoliosToRefresh)
                    {
                        allPortfolios.Add(folio);
                    }

                    foreach (var portfolio in allPortfolios)
                    {
                        cellsToRefresh.AddRange(_portfolioSubscriptions.Get(portfolio));
                    }
                }

                foreach (var cell in cellsToRefresh)
                {
                    args.PortfolioValues.Add((cell.FolioId, cell.ColumnName), cell.GetValue());
                }
            }

            void RefreshPositionCells()
            {
                var cellsToRefresh = new List<PositionCellValue>();

                if (arg2.RedrawAll)
                {
                    cellsToRefresh.AddRange(_positionSubscriptions.GetCells());
                }
                else
                {
                    foreach (var position in arg2.PositionsToRedraw)
                    {
                        cellsToRefresh.AddRange(_positionSubscriptions.Get(position));
                    }
                }

                foreach (var cell in cellsToRefresh)
                {
                    args.PositionValues.Add((cell.PositionId, cell.ColumnName), cell.GetValue());
                }
            }

            RefreshPortfolioCells();

            RefreshPositionCells();

            DataAvailable?.Invoke(this, args);
        }

        //public bool IsBusy { get; private set; }

        //public bool LoadUnloadedPortfolios { get; set; } = false;

        //public TimeSpan ElapsedTimeOfLastCall => _lastRefreshTimer;

        //public object GetPortfolioValue(int portfolioId, string column)
        //{
        //    object result = "#N/A";

        //    _context.Send(state => {

        //        try
        //        {
        //            //LoadRequiredData();

        //            result = GetPortfolioValueUI(portfolioId, column);
        //        }
        //        catch (Exception ex)
        //        {
        //            result = ex.Message;
        //        }

        //    }, null);

        //    return result;
        //}

        //public object GetPositionValue(int positionId, string column)
        //{
        //    object result = "#N/A";

        //    _context.Send(state => {

        //        try
        //        {
        //            //LoadRequiredData();

        //            result = GetPositionValueUI(positionId, column);
        //        }
        //        catch(Exception ex)
        //        {
        //            result = ex.Message;
        //        }

        //    }, null);

        //    return result;
        //}

        public object GetSystemValue(SystemProperty property)
        {
            object result = "#N/A";

            _context.Send(state => {

                try
                {
                    result = GetSystemValueUI(property);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }, null);

            return result;
        }

        public void GetSystemValues(IDictionary<SystemProperty, object> values)
        {
            _context.Send(state => {

                foreach (var key in values.Keys.ToList())
                {
                    try
                    {
                        values[key] = GetSystemValueUI(key);
                    }
                    catch (Exception ex)
                    {
                        values[key] = ex.Message;
                    }
                }

            }, null);
        }

        //public void GetPositionValues(IDictionary<(int positionId, string column), object> values)
        //{
        //    _context.Send(state => {

        //        TimeUpdate(() => {

        //            LoadRequiredData();

        //            foreach (var key in values.Keys.ToList())
        //            {
        //                try
        //                {
        //                    values[key] = GetPositionValueUI(key.positionId, key.column);
        //                }
        //                catch (Exception ex)
        //                {
        //                    values[key] = ex.Message;
        //                }
        //            }

        //        });

        //    }, null);
        //}

        //public void GetPortfolioValues(IDictionary<(int positionId, string column), object> values)
        //{
        //    _context.Send(state => {

        //        TimeUpdate(() => {

        //            LoadRequiredData();

        //            foreach (var key in values.Keys.ToList())
        //            {
        //                try
        //                {
        //                    values[key] = GetPortfolioValueUI(key.positionId, key.column);
        //                }
        //                catch (Exception ex)
        //                {
        //                    values[key] = ex.Message;
        //                }
        //            }

        //        });

        //    }, null);
        //}

        public List<int> GetPositions(int folioId, Positions positions)
        {
            var results = new List<int>();

            _context.Send(state => {

                using (var portfolio = CSMPortfolio.GetCSRPortfolio(folioId))
                {
                    if (!portfolio.IsLoaded())
                        return;
                    
                    GetPositionsUI(folioId, positions, results);
                }

            }, null);

            return results;
        }

        private void GetPositionsUI(int folioId, Positions positions, List<int> results)
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
                            case Positions.All:
                                results.Add(position.GetIdentifier());
                                break;

                            case Positions.Open:
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

        private object GetSystemValueUI(SystemProperty property)
        {
            switch (property)
            {
                case SystemProperty.PortfolioDate: return CSMPortfolio.GetPortfolioDate().GetDateTime();
            }

            throw new ApplicationException($"Unknown property {property}");
        }

        //private CSMPortfolioColumn GetPortfolioColumn(string column)
        //{
        //    if(!_columnCache.ContainsKey(column))
        //    {
        //        _columnCache.Add(column, CSMPortfolioColumn.GetCSRPortfolioColumn(column));
        //    }

        //    return _columnCache[column];
        //}

        //private object GetPortfolioValueUI(int portfolioId, string column)
        //{
        //    var cv = new SSMCellValue();

        //    using (var portfolio = CSMPortfolio.GetCSRPortfolio(portfolioId))
        //    using (var cs = new SSMCellStyle())
        //    {
        //        if (portfolio is null)
        //        {
        //            return $"The requested Portfolio '{portfolioId}' cannot be found";
        //        }

        //        var portfolioColumn = GetPortfolioColumn(column);

        //        if (portfolioColumn is null)
        //        {
        //            return $"The requested Portfolio Column '{column}' cannot be found";
        //        }

        //        if (!portfolio.IsLoaded())
        //        {
        //            if (LoadUnloadedPortfolios)
        //            {
        //                _portfoliosToLoad.Enqueue(portfolioId);
        //                return "Loading the portfolio (F8)... please wait";
        //            }
        //            else
        //            {
        //                return $"The requested portfolio '{portfolioId}' is not loaded. Please load in the FusionInvest client.";
        //            }
        //        }
        //        else
        //        {
        //            portfolioColumn.GetPortfolioCell(portfolioId, portfolioId, null, ref cv, cs, true);

        //            return cv.ExtractValueFromSophisCell(cs);
        //        }
        //    }
        //}

        //private object GetPositionValueUI(int positionId, string column)
        //{
        //    var cv = new SSMCellValue();

        //    using (var position = CSMPosition.GetCSRPosition(positionId))
        //    using (var cs = new SSMCellStyle())
        //    {
        //        if (position is null)
        //        {
        //            return $"The requested position '{positionId}' is either not loaded or does not exist. Please load the positions portfolio in Sophis.";
        //        }

        //        var portfolioColumn = GetPortfolioColumn(column);
        //        if (portfolioColumn is null)
        //        {
        //            return $"The requested Portfolio Column '{column}' cannot be found";
        //        }

        //        using (var portfolio = position.GetPortfolio())
        //        {
        //            if (!portfolio.IsLoaded())
        //            {
        //                if (LoadUnloadedPortfolios)
        //                {
        //                    _portfoliosToLoad.Enqueue(portfolio.GetCode());
        //                    return "Loading the portfolio. Please wait...";
        //                }
        //                else
        //                {
        //                    return $"The requested portfolio '{portfolio.GetCode()}' is not loaded. Please load in the FusionInvest client.";
        //                }
        //            }
        //            else
        //            {
        //                portfolioColumn.GetPositionCell(position, position.GetPortfolioCode(), position.GetPortfolioCode(), null, 0, position.GetInstrumentCode(), ref cv, cs, true);

        //                return cv.ExtractValueFromSophisCell(cs);
        //            }
        //        }
        //    }
        //}

        //private void LoadRequiredData()
        //{
        //    if (LoadUnloadedPortfolios)
        //    {
        //        if (_portfoliosToLoad.Count > 0)
        //        {
        //            while (_portfoliosToLoad.Count > 0)
        //            {
        //                int portfolioId = _portfoliosToLoad.Dequeue();
        //                using (var portfolio = CSMPortfolio.GetCSRPortfolio(portfolioId))
        //                {
        //                    EnsurePortfolioLoaded(portfolio);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void EnsurePortfolioLoaded(CSMPortfolio portfolio)
        //{
        //    if (!portfolio.IsLoaded())
        //    {
        //        IsBusy = true;

        //        portfolio.Load();
        //        portfolio.Compute();

        //        IsBusy = false;
        //    }
        //}

        //private void TimeUpdate(Action action)
        //{
        //    var timer = Stopwatch.StartNew();

        //    action.Invoke();

        //    timer.Stop();

        //    _lastRefreshTimer = timer.Elapsed;
        //}

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

            }, null);

        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            _context.Post(state => {

                _systemValueSubscriptions.Add(property, GetSystemValueProperty(property));

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
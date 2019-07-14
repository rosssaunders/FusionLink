//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using RxdSolutions.FusionLink.Interface;
using RxdSolutions.FusionLink.Listeners;
using RxdSolutions.FusionLink.Services;
using sophis.instrument;
using sophis.portfolio;
using sophis.utils;

namespace RxdSolutions.FusionLink.Provider
{
    public class FusionDataServiceProvider : IDataServerProvider, IDisposable
    {
        private readonly string _className = nameof(FusionDataServiceProvider);

        private readonly Dispatcher _context;
        private readonly IGlobalFunctions _globalFunctions;
        private readonly IPortfolioListener _portfolioListener;
        private readonly IPositionListener _positionListener;
        private readonly ITransactionListener _transactionListener;
        private readonly PositionService _positionService;
        private readonly InstrumentService _instrumentService;
        private readonly CurveService _curveService;

        private readonly Subscriptions<PortfolioCellValue, string> _portfolioCellSubscriptions;
        private readonly Subscriptions<PortfolioPropertyValue, PortfolioProperty> _portfolioPropertySubscriptions;
        private readonly Subscriptions<PositionCellValue, string> _positionCellSubscriptions;
        private readonly Dictionary<SystemProperty, SystemValue> _systemValueSubscriptions;

        private readonly CSMExtraction _mainExtraction;

        private readonly List<int> _portfolioComputedIds;
        private int _portfolioRefreshCount = 0;

        public bool IsRunning { get; private set; }

        public event EventHandler<DataAvailableEventArgs> DataAvailable;

        public bool HasSubscriptions 
        {
            get 
            {
                return _portfolioCellSubscriptions.Count > 0 || 
                       _positionCellSubscriptions.Count > 0 || 
                       _systemValueSubscriptions.Count > 0 ||
                       _portfolioPropertySubscriptions.Count > 0;
            }
        }

        public FusionDataServiceProvider(IGlobalFunctions globalFunctions, 
                                         IPortfolioListener portfolioListener,
                                         IPositionListener positionListener,
                                         ITransactionListener transactionListener,
                                         PositionService positionService,
                                         InstrumentService instrumentService,
                                         CurveService curveService)
        {
            _context = Dispatcher.CurrentDispatcher;
            _globalFunctions = globalFunctions;
            _portfolioListener = portfolioListener;
            _positionListener = positionListener;
            _transactionListener = transactionListener;
            _positionService = positionService;
            _instrumentService = instrumentService;
            _curveService = curveService;
            _mainExtraction = sophis.globals.CSMExtraction.gMain();

            _portfolioCellSubscriptions = new Subscriptions<PortfolioCellValue, string>((i, s) => new PortfolioCellValue(i, s, _mainExtraction));
            _positionCellSubscriptions = new Subscriptions<PositionCellValue, string>((i, s) => new PositionCellValue(i, s, _mainExtraction));
            _portfolioPropertySubscriptions = new Subscriptions<PortfolioPropertyValue, PortfolioProperty>((i, s) => new PortfolioPropertyValue(i, s));
            _systemValueSubscriptions = new Dictionary<SystemProperty, SystemValue>();
            _portfolioComputedIds = new List<int>();

            _globalFunctions.PortfolioCalculationEnded += GlobalFunctions_PortfolioCalculationEnded;
            _portfolioListener.PortfolioChanged += PortfolioListener_PortfolioChanged;
            _positionListener.PositionChanged += PositionListener_PositionChanged;
            _transactionListener.TransactionChanged += TransactionListener_TransactionChanged;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public List<int> GetPositions(int folioId, PositionsToRequest positions)
        {
            return _context.Invoke(() => {

                try
                {
                    return _positionService.GetPositions(folioId, positions);
                }
                catch(PortfolioNotLoadedException e)
                {
                    CSMLog.Write(_className, "GetPositions", CSMLog.eMVerbosity.M_verbose, e.ToString());
                    throw;
                }
                catch(PortfolioNotFoundException e)
                {
                    CSMLog.Write(_className, "GetPositions", CSMLog.eMVerbosity.M_verbose, e.ToString());
                    throw;
                }
                catch (Exception e)
                {
                    CSMLog.Write(_className, "GetPositions", CSMLog.eMVerbosity.M_error, e.ToString());
                    throw;
                }

            });
        }

        public List<PriceHistory> GetPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            return _context.Invoke(() => {

                try
                {
                    var instrumentId = CSMInstrument.GetCode(reference);

                    return _instrumentService.GetPriceHistory(instrumentId, startDate, endDate);
                }
                catch(InstrumentNotFoundException e)
                {
                    CSMLog.Write(_className, "GetPriceHistory", CSMLog.eMVerbosity.M_verbose, e.ToString());
                    throw;
                }
                catch (Exception e)
                {
                    CSMLog.Write(_className, "GetPriceHistory", CSMLog.eMVerbosity.M_error, e.ToString());
                    throw;
                }

            });
        }

        public List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            return _context.Invoke(() => {

                try
                {
                    return _instrumentService.GetPriceHistory(instrumentId, startDate, endDate);
                }
                catch (InstrumentNotFoundException e)
                {
                    CSMLog.Write(_className, "GetPriceHistory", CSMLog.eMVerbosity.M_verbose, e.ToString());
                    throw;
                }
                catch (Exception e)
                {
                    CSMLog.Write(_className, "GetPriceHistory", CSMLog.eMVerbosity.M_error, e.ToString());
                    throw;
                }
            });
        }

        public List<CurvePoint> GetCurvePoints(string currency, string family, string reference)
        {
            return _context.Invoke(() => {

                try
                {
                    return _curveService.GetCurvePoints(currency, family, reference);
                }
                catch(CurrencyNotFoundException e)
                {
                    CSMLog.Write(_className, "GetCurvePoints", CSMLog.eMVerbosity.M_verbose, e.ToString());
                    throw;
                }
                catch (CurveFamilyFoundException e)
                {
                    CSMLog.Write(_className, "GetCurvePoints", CSMLog.eMVerbosity.M_verbose, e.ToString());
                    throw;
                }
                catch (CurveNotFoundException e)
                {
                    CSMLog.Write(_className, "GetCurvePoints", CSMLog.eMVerbosity.M_verbose, e.ToString());
                    throw;
                }
                catch (Exception e)
                {
                    CSMLog.Write(_className, "GetCurvePoints", CSMLog.eMVerbosity.M_error, e.ToString());
                    throw;
                }
            });
        }

        public void SubscribeToPortfolio(int portfolioId, string column)
        {
            var op = _context.InvokeAsync(() => {

                _portfolioCellSubscriptions.Add(portfolioId, column);
                var dp = _portfolioCellSubscriptions.Get(portfolioId, column);

                try
                {
                    var da = new DataAvailableEventArgs();
                    da.PortfolioValues.Add((portfolioId, column), dp.GetValue());

                    DataAvailable?.Invoke(this, da);

                    dp.Error = null;
                }
                catch(Exception ex)
                {
                    dp.Error = ex;

                    CSMLog.Write(_className, "SubscribeToPortfolio", CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void SubscribeToPosition(int positionId, string column)
        {
            var op = _context.InvokeAsync(() => {

                _positionCellSubscriptions.Add(positionId, column);
                var dp = _positionCellSubscriptions.Get(positionId, column);

                try
                {
                    var da = new DataAvailableEventArgs();
                    da.PositionValues.Add((positionId, column), dp.GetValue());

                    DataAvailable?.Invoke(this, da);

                    dp.Error = null;
                }
                catch (Exception ex)
                {
                    dp.Error = ex;

                    CSMLog.Write(_className, "SubscribeToPosition", CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            var op = _context.InvokeAsync(() => {

                _systemValueSubscriptions.Add(property, GetSystemValueProperty(property));
                var dp = _systemValueSubscriptions[property];

                try
                {
                    var da = new DataAvailableEventArgs();
                    da.SystemValues.Add(property, dp.GetValue());

                    DataAvailable?.Invoke(this, da);
                }
                catch (Exception ex)
                {
                    dp.Error = ex;
                    CSMLog.Write(_className, "SubscribeToSystemValue", CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void SubscribeToPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            var op = _context.InvokeAsync(() => {

                _portfolioPropertySubscriptions.Add(portfolioId, property);
                var dp = _portfolioPropertySubscriptions.Get(portfolioId, property);

                try
                {
                    var da = new DataAvailableEventArgs();
                    da.PortfolioProperties.Add((portfolioId, property), dp.GetValue());

                    DataAvailable?.Invoke(this, da);
                }
                catch (Exception ex)
                {
                    dp.Error = ex;
                    CSMLog.Write(_className, "SubscribeToPortfolioProperty", CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void UnsubscribeFromPortfolio(int portfolioId, string column)
        {
            var op = _context.InvokeAsync(() => {

                try
                {
                    _portfolioCellSubscriptions.Remove(portfolioId, column);
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, "UnsubscribeToPortfolio", CSMLog.eMVerbosity.M_error, ex.ToString());
                }
                
            }, DispatcherPriority.Normal);
        }

        public void UnsubscribeFromPosition(int positionId, string column)
        {
            var op = _context.InvokeAsync(() => {

                try
                {
                    _positionCellSubscriptions.Remove(positionId, column);
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, "UnsubscribeToPosition", CSMLog.eMVerbosity.M_error, ex.ToString());
                }
                
            }, DispatcherPriority.Normal);
        }

        public void UnsubscribeFromSystemValue(SystemProperty property)
        {
            var op = _context.InvokeAsync(() => {

                try
                {
                    _systemValueSubscriptions.Remove(property);
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, "UnsubscribeToSystemValue", CSMLog.eMVerbosity.M_error, ex.ToString());
                }
                
            }, DispatcherPriority.Normal);
        }

        public void UnsubscribeFromPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            var op = _context.InvokeAsync(() => {

                try
                {
                    _portfolioPropertySubscriptions.Remove(portfolioId, property);
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, "UnsubscribeToPortfolioProperty", CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        
        private void GlobalFunctions_PortfolioCalculationEnded(object sender, PortfolioCalculationEndedEventArgs e)
        {
            if (IsRunning && HasSubscriptions)
            {
                switch (e.InPortfolioCalculation)
                {
                    case sophis.misc.CSMGlobalFunctions.eMPortfolioCalculationType.M_pcFullCalculation:

                        _portfolioComputedIds.Add(e.FolioId);

                        _context.InvokeAsync(() =>
                        {
                            try
                            {
                                if (_portfolioComputedIds.Count == 0)
                                    return;

                                ComputePortfolios(_portfolioComputedIds);

                                _portfolioComputedIds.Clear();

                                RefreshData();
                            }
                            catch (Exception ex)
                            {
                                CSMLog.Write(_className, "GlobalFunctions_PortfolioCalculationEnded", CSMLog.eMVerbosity.M_error, ex.ToString());
                            }

                        }, DispatcherPriority.ApplicationIdle);

                        break;

                    case sophis.misc.CSMGlobalFunctions.eMPortfolioCalculationType.M_pcJustSumming:

                        switch (CSMPreference.GetAutomaticComputatingType())
                        {
                            case eMAutomaticComputingType.M_acQuotation:
                            case eMAutomaticComputingType.M_acLast:
                            case eMAutomaticComputingType.M_acNothing:
                                break;

                            case eMAutomaticComputingType.M_acPortfolioWithoutPNL:
                            case eMAutomaticComputingType.M_acPortfolioOnlyPNL:
                            case eMAutomaticComputingType.M_acFolio:

                                try
                                {
                                    var id = e.Extraction.GetInternalID();

                                    if (id == 1)
                                    {
                                        _portfolioRefreshCount++;

                                        _context.InvokeAsync(() =>
                                        {
                                            try
                                            {
                                                if (_portfolioRefreshCount == 0)
                                                    return;

                                                RefreshData();

                                                _portfolioRefreshCount = 0;
                                            }
                                            catch (Exception ex)
                                            {
                                                CSMLog.Write(_className, "GlobalFunctions_PortfolioCalculationEnded", CSMLog.eMVerbosity.M_error, ex.ToString());
                                            }

                                        }, DispatcherPriority.ApplicationIdle);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CSMLog.Write(_className, "GlobalFunctions_PortfolioCalculationEnded", CSMLog.eMVerbosity.M_error, ex.ToString());
                                }

                                break;
                        }

                        //Do Nothing for now.
                        break;

                    case sophis.misc.CSMGlobalFunctions.eMPortfolioCalculationType.M_pcNotInPortfolio:
                        return;
                }
            }
        }

        private void ComputePortfolios(List<int> skipPortfolio)
        {
            var portfolios = new HashSet<int>();

            foreach (var c in _portfolioCellSubscriptions.GetCells())
            {
                if(c.Portfolio is object)
                {
                    portfolios.Add(c.FolioId);
                }
            }
                
            foreach (var c in _positionCellSubscriptions.GetCells())
            {
                if(c.Position is object)
                {
                    portfolios.Add(c.Position.GetPortfolioCode());
                }
            }

            int FindRootLoadedPortfolio(CSMPortfolio portfolio)
            {
                int code = portfolio.GetCode();
                int parentCode = portfolio.GetParentCode();

                if (code == parentCode)
                    return code;

                if (parentCode == 1 || parentCode == 0)
                    return 1;

                using (var parentPortfolio = CSMPortfolio.GetCSRPortfolio(parentCode))
                {
                    if(parentPortfolio.IsLoaded())
                    {
                        return FindRootLoadedPortfolio(parentPortfolio);
                    }
                    else
                    {
                        return portfolio.GetCode();
                    }
                }
            }

            var rootPortfolios = new HashSet<int>();
            foreach(int id in portfolios)
            {
                using (var portfolio = CSMPortfolio.GetCSRPortfolio(id))
                {
                    if (portfolio.IsLoaded())
                    {
                        rootPortfolios.Add(FindRootLoadedPortfolio(portfolio));
                    }
                }
            }

            foreach (int id in rootPortfolios)
            {
                if (skipPortfolio.Contains(id))
                    continue;

                using (var portfolio = CSMPortfolio.GetCSRPortfolio(id))
                {
                    portfolio.Compute();
                }
            }
        }

        private void RefreshData()
        {
            var args = new DataAvailableEventArgs();

            void RefreshPortfolioCells()
            {
                foreach (var cell in _portfolioCellSubscriptions.GetCells())
                {
                    args.PortfolioValues.Add((cell.FolioId, cell.ColumnName), cell.GetValue());
                }
            }

            void RefreshPositionCells()
            {
                foreach (var cell in _positionCellSubscriptions.GetCells())
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

            var sw = Stopwatch.StartNew();

            RefreshPortfolioCells();

            RefreshPositionCells();

            RefreshSystemValues();

            sw.Stop();

            args.TimeTaken = sw.Elapsed;

            DataAvailable?.Invoke(this, args);
        }

        private void PortfolioListener_PortfolioChanged(object sender, PortfolioChangedEventArgs e)
        {
            RefreshPortfolio(e.PortfolioId, e.IsLocal);
        }

        private void PositionListener_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            RefreshPosition(e.PositionId, e.IsLocal);
        }

        private void TransactionListener_TransactionChanged(object sender, TransactionChangedEventArgs e)
        {
            RefreshPosition(e.PositionId, e.IsLocal);
        }

        private void RefreshPortfolio(int portfolioId, bool isLocal)
        {
            try
            {
                void NotifyDataChanged()
                {
                    foreach (var cell in _portfolioCellSubscriptions.GetCells().ToList())
                    {
                        if (cell.FolioId == portfolioId)
                        {
                            _portfolioCellSubscriptions.Remove(cell.FolioId, cell.ColumnName);
                            _portfolioCellSubscriptions.Add(portfolioId, cell.ColumnName);
                        }
                    }

                    foreach (var cell in _portfolioPropertySubscriptions.GetCells().ToList())
                    {
                        if (cell.FolioId == portfolioId)
                        {
                            _portfolioPropertySubscriptions.Remove(cell.FolioId, cell.Property);
                            _portfolioPropertySubscriptions.Add(portfolioId, cell.Property);
                        }
                    }

                    var args = new DataAvailableEventArgs();

                    foreach (var value in _portfolioCellSubscriptions.Get(portfolioId))
                    {
                        args.PortfolioValues.Add((value.FolioId, value.ColumnName), value.GetValue());
                    }

                    foreach (var value in _portfolioPropertySubscriptions.Get(portfolioId))
                    {
                        args.PortfolioProperties.Add((value.FolioId, value.Property), value.GetValue());
                    }

                    DataAvailable?.Invoke(this, args);
                }

                if (isLocal)
                {
                    //There must be a Sophis API call which does the same work without the risk of deadlock.
                    var yield = Dispatcher.Yield(DispatcherPriority.ApplicationIdle);
                    yield.GetAwaiter().OnCompleted(() =>
                    {
                        NotifyDataChanged();
                    });
                }
                else
                {
                    NotifyDataChanged();
                }
            }
            catch (Exception ex)
            {
                CSMLog.Write(_className, "RefreshPortfolio", CSMLog.eMVerbosity.M_error, ex.ToString());
            }
        }

        private void RefreshPosition(int positionId, bool isLocal)
        {
            try
            {
                void NotifyDataChanged()
                {
                    foreach (var cell in _positionCellSubscriptions.GetCells().ToList())
                    {
                        if (cell.PositionId == positionId)
                        {
                            _positionCellSubscriptions.Remove(cell.PositionId, cell.ColumnName);
                            _positionCellSubscriptions.Add(positionId, cell.ColumnName);
                        }
                    }

                    var args = new DataAvailableEventArgs();

                    foreach (var value in _positionCellSubscriptions.Get(positionId))
                    {
                        args.PositionValues.Add((value.PositionId, value.ColumnName), value.GetValue());
                    }

                    DataAvailable?.Invoke(this, args);
                }

                if (isLocal)
                {
                    var yield = Dispatcher.Yield(DispatcherPriority.ApplicationIdle);
                    yield.GetAwaiter().OnCompleted(() =>
                    {
                        NotifyDataChanged();
                    });
                }
                else
                {
                    NotifyDataChanged();
                }
            }
            catch (Exception ex)
            {
                CSMLog.Write(_className, "RefreshPosition", CSMLog.eMVerbosity.M_error, ex.ToString());
            }
        }

        private SystemValue GetSystemValueProperty(SystemProperty property)
        {
            switch (property)
            {
                case SystemProperty.PortfolioDate:
                    return new PortfolioDateValue();

                case SystemProperty.IsRealTimeEnabled:
                    return new IsRealTimeEnabledValue();
            }

            throw new InvalidOperationException();
        }

        public void RequestCalculate()
        {
            //We don't want to wait for a response for this one to stop freezing the calling app or timing out the connection
            _context.InvokeAsync(() => {

                try
                {
                    ComputePortfolios(new List<int>() { -1 });

                    RefreshData();
                }
                catch(Exception ex)
                {
                    CSMLog.Write(_className, "RequestCalculate", CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.ApplicationIdle);
        }

        public void LoadPositions()
        {
            //We don't want to wait for a response for this one to stop freezing the calling app or timing out the connection
            _context.InvokeAsync(() => {

                try
                {
                    var portfolios = new Dictionary<int, CSMPortfolio>();
                    foreach (var portfolioSubscription in _portfolioCellSubscriptions.GetCells())
                    {
                        portfolios[portfolioSubscription.FolioId] = portfolioSubscription.Portfolio;
                    }

                    foreach (var portfolio in portfolios.Values)
                    {
                        if (portfolio is object)
                        {
                            if (!portfolio.IsLoaded())
                            {
                                portfolio.Load();
                            }
                        }
                    }

                    RequestCalculate();
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, "LoadPositions", CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.ApplicationIdle);
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _mainExtraction.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
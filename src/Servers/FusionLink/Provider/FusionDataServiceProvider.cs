//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Windows.Threading;
using RxdSolutions.FusionLink.Interface;
using RxdSolutions.FusionLink.Services;
using sophis.instrument;
using sophis.portfolio;
using sophis.utils;

namespace RxdSolutions.FusionLink
{
    public class FusionDataServiceProvider : IDataServerProvider
    {
        private readonly string _className = nameof(FusionDataServiceProvider);

        private readonly Dispatcher _context;
        private readonly IGlobalFunctions _globalFunctions;
        private readonly IPortfolioListener _portfolioListener;
        private readonly PositionService _positionService;
        private readonly InstrumentService _instrumentService;
        private readonly CurveService _curveService;

        private readonly Subscriptions<PortfolioCellValue, string> _portfolioCellSubscriptions;
        private readonly Subscriptions<PortfolioPropertyValue, PortfolioProperty> _portfolioPropertySubscriptions;
        private readonly Subscriptions<PositionCellValue, string> _positionCellSubscriptions;
        private readonly Dictionary<SystemProperty, SystemValue> _systemValueSubscriptions;

        //Avoid infinite loops
        private int _computeCount;

        //Optimize the data refreshing
        private int _dataRefreshRequests = 0;

        public bool IsRunning { get; private set; }

        public event EventHandler<DataAvailableEventArgs> DataAvailable;

        public bool HasSubscriptions 
        {
            get 
            {
                return _portfolioCellSubscriptions.Count > 0 || _positionCellSubscriptions.Count > 0 || _systemValueSubscriptions.Count > 0;
            }
        }

        public FusionDataServiceProvider(IGlobalFunctions globalFunctions, 
                                         IPortfolioListener portfolioListener,
                                         PositionService positionService,
                                         InstrumentService instrumentService,
                                         CurveService curveService)
        {
            _context = Dispatcher.CurrentDispatcher;
            _globalFunctions = globalFunctions;
            _portfolioListener = portfolioListener;
            _positionService = positionService;
            _instrumentService = instrumentService;
            _curveService = curveService;

            _portfolioCellSubscriptions = new Subscriptions<PortfolioCellValue, string>((i, s) => new PortfolioCellValue(i, s));
            _positionCellSubscriptions = new Subscriptions<PositionCellValue, string>((i, s) => new PositionCellValue(i, s));
            _portfolioPropertySubscriptions = new Subscriptions<PortfolioPropertyValue, PortfolioProperty>((i, s) => new PortfolioPropertyValue(i, s));
            _systemValueSubscriptions = new Dictionary<SystemProperty, SystemValue>();

            _globalFunctions.PortfolioCalculationEnded += GlobalFunctions_PortfolioCalculationEnded;
            _portfolioListener.PortfolioChanged += PortfolioListener_PortfolioChanged;
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

                try
                {
                    _portfolioCellSubscriptions.Add(portfolioId, column);

                    var da = new DataAvailableEventArgs();
                    da.PortfolioValues.Add((portfolioId, column), _portfolioCellSubscriptions.Get(portfolioId, column).GetValue());

                    DataAvailable?.Invoke(this, da);
                }
                catch(Exception ex)
                {
                    CSMLog.Write(_className, "SubscribeToPortfolio", CSMLog.eMVerbosity.M_error, ex.ToString());
                    throw;
                }

            }, DispatcherPriority.Normal);

            op.Wait();
        }

        public void SubscribeToPosition(int positionId, string column)
        {
            var op = _context.InvokeAsync(() => {

                try
                {
                    _positionCellSubscriptions.Add(positionId, column);

                    var da = new DataAvailableEventArgs();
                    da.PositionValues.Add((positionId, column), _positionCellSubscriptions.Get(positionId, column).GetValue());

                    DataAvailable?.Invoke(this, da);
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, "SubscribeToPosition", CSMLog.eMVerbosity.M_error, ex.ToString());
                    throw;
                }

            }, DispatcherPriority.Normal);

            op.Wait();
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            var op = _context.InvokeAsync(() => {

                try
                {
                    _systemValueSubscriptions.Add(property, GetSystemValueProperty(property));

                    var da = new DataAvailableEventArgs();
                    da.SystemValues.Add(property, _systemValueSubscriptions[property].GetValue());

                    DataAvailable?.Invoke(this, da);
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, "SubscribeToSystemValue", CSMLog.eMVerbosity.M_error, ex.ToString());
                    throw;
                }

            }, DispatcherPriority.Normal);

            op.Wait();
        }

        public void SubscribeToPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            var op = _context.InvokeAsync(() => {

                try
                {
                    _portfolioPropertySubscriptions.Add(portfolioId, property);

                    var da = new DataAvailableEventArgs();
                    da.PortfolioProperties.Add((portfolioId, property), _portfolioPropertySubscriptions.Get(portfolioId, property).GetValue());

                    DataAvailable?.Invoke(this, da);
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, "SubscribeToPortfolioProperty", CSMLog.eMVerbosity.M_error, ex.ToString());
                    throw;
                }

            }, DispatcherPriority.Normal);

            op.Wait();            
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
                    throw;
                }
                
            }, DispatcherPriority.ApplicationIdle);

            op.Wait();
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
                    throw;
                }
                
            }, DispatcherPriority.ApplicationIdle);

            op.Wait();
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
                    throw;
                }
                
            }, DispatcherPriority.ApplicationIdle);

            op.Wait();
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
                    throw;
                }

            }, DispatcherPriority.ApplicationIdle);

            op.Wait();
        }

        private void GlobalFunctions_PortfolioCalculationEnded(object sender, PortfolioCalculationEndedEventArgs e)
        {
            if (_computeCount > 0)
            {
                //It was us that triggered this compute
                _computeCount--;
                return;
            }

            if (IsRunning && HasSubscriptions)
            {
                switch (e.InPortfolioCalculation)
                {
                    case sophis.misc.CSMGlobalFunctions.eMPortfolioCalculationType.M_pcFullCalculation:

                        try
                        {
                            ComputePortfolios(e.FolioId);
                        }
                        catch(Exception ex)
                        {
                            CSMLog.Write(_className, "GlobalFunctions_PortfolioCalculationEnded", CSMLog.eMVerbosity.M_error, ex.ToString());
                        }

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
                                        _dataRefreshRequests++;
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

                try
                {
                    RefreshData();
                }
                catch(Exception ex)
                {
                    CSMLog.Write(_className, "GlobalFunctions_PortfolioCalculationEnded", CSMLog.eMVerbosity.M_error, ex.ToString());
                }
            }
        }

        private void ComputePortfolios(int skipPortfolio)
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
                var parentCode = portfolio.GetParentCode();
                
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
            foreach(var id in portfolios)
            {
                using (var portfolio = CSMPortfolio.GetCSRPortfolio(id))
                {
                    if (portfolio.IsLoaded())
                    {
                        rootPortfolios.Add(FindRootLoadedPortfolio(portfolio));
                    }
                }
            }

            foreach (var id in rootPortfolios)
            {
                if (id == skipPortfolio)
                    continue;

                using (var portfolio = CSMPortfolio.GetCSRPortfolio(id))
                {
                    _computeCount++;

                    portfolio.Compute();
                }
            }

            _dataRefreshRequests++;
        }

        private void RefreshData()
        {
            if (_computeCount > 0)
                return;

            if (_dataRefreshRequests == 0)
                return;

            _dataRefreshRequests = 0;

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

            RefreshPortfolioCells();

            RefreshPositionCells();

            RefreshSystemValues();

            DataAvailable?.Invoke(this, args);
        }

        private void PortfolioListener_PortfolioChanged(object sender, PortfolioChangedEventArgs e)
        {
            try
            {
                if (e.IsLocal)
                {
                    //There must be a Sophis API call which does the same work without the risk of deadlock.
                    var yield = Dispatcher.Yield();
                    yield.GetAwaiter().OnCompleted(() =>
                    {
                        var args = new DataAvailableEventArgs();

                        foreach (var value in _portfolioPropertySubscriptions.GetCells())
                        {
                            args.PortfolioProperties.Add((value.FolioId, value.Property), value.GetValue());
                        }

                        DataAvailable?.Invoke(this, args);
                    });
                }
                else
                {
                    var args = new DataAvailableEventArgs();

                    foreach (var value in _portfolioPropertySubscriptions.GetCells())
                    {
                        args.PortfolioProperties.Add((value.FolioId, value.Property), value.GetValue());
                    }

                    DataAvailable?.Invoke(this, args);
                }
            }
            catch(Exception ex)
            {
                CSMLog.Write(_className, "PortfolioListener_PortfolioChanged", CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    ComputePortfolios(-1);

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
    }
}
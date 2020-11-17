//  Copyright (c) RXD Solutions. All rights reserved.
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
using sophis.static_data;
using sophis.utils;

namespace RxdSolutions.FusionLink.Provider
{
    public class FusionRealTimeProvider : IRealTimeProvider, IDisposable
    {
        private readonly string _className = nameof(FusionRealTimeProvider);

        private readonly Dispatcher _context;
        private readonly IGlobalFunctions _globalFunctions;
        private readonly IPortfolioListener _portfolioListener;
        private readonly IPositionListener _positionListener;
        private readonly ITransactionListener _transactionListener;
        private readonly IInstrumentListener _instrumentStaticListener;
        private readonly IInstrumentListener _instrumentMarketDataListener;
        private readonly InstrumentService _instrumentService;
        private readonly CurrencyService _currencyService;
        private readonly Subscriptions<PortfolioCellValue, string, int> _portfolioCellSubscriptions;
        private readonly Subscriptions<PortfolioPropertyValue, PortfolioProperty, int> _portfolioPropertySubscriptions;
        private readonly Subscriptions<InstrumentPropertyValue, (object Reference, string Property), int> _instrumentPropertySubscriptions;
        private readonly Subscriptions<CurrencyPropertyValue, (object Reference, string Property), int> _currencyPropertySubscriptions;
        private readonly Subscriptions<PositionCellValue, string, int> _positionCellSubscriptions;
        private readonly Subscriptions<FlatPositionCellValue, string, (int, int)> _flatPositionCellSubscriptions;
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
                       _flatPositionCellSubscriptions.Count > 0 ||
                       _systemValueSubscriptions.Count > 0 ||
                       _portfolioPropertySubscriptions.Count > 0 ||
                       _instrumentPropertySubscriptions.Count > 0 ||
                       _currencyPropertySubscriptions.Count > 0;
            }
        }

        public FusionRealTimeProvider(IGlobalFunctions globalFunctions, 
                                      IPortfolioListener portfolioListener,
                                      IPositionListener positionListener,
                                      ITransactionListener transactionListener,
                                      IInstrumentListener instrumentStaticListener,
                                      IInstrumentListener instrumentMarketDataListener,
                                      InstrumentService instrumentService,
                                      CurrencyService currencyService)
        {
            _context = Dispatcher.CurrentDispatcher;
            _globalFunctions = globalFunctions;
            _portfolioListener = portfolioListener;
            _positionListener = positionListener;
            _transactionListener = transactionListener;
            _instrumentStaticListener = instrumentStaticListener;
            _instrumentMarketDataListener = instrumentMarketDataListener;
            _instrumentService = instrumentService;
            _currencyService = currencyService;

            _mainExtraction = sophis.globals.CSMExtraction.gMain();

            _portfolioCellSubscriptions = new Subscriptions<PortfolioCellValue, string, int>((id, column) => new PortfolioCellValue(id, column, _mainExtraction));
            _positionCellSubscriptions = new Subscriptions<PositionCellValue, string, int>((id, column) => new PositionCellValue(id, column, _mainExtraction));
            _flatPositionCellSubscriptions = new Subscriptions<FlatPositionCellValue, string, (int portfolioId, int instrumentId)>((ids, column) => new FlatPositionCellValue(ids.portfolioId, ids.instrumentId, column, _mainExtraction));
            _portfolioPropertySubscriptions = new Subscriptions<PortfolioPropertyValue, PortfolioProperty, int>((id, property) => new PortfolioPropertyValue(id, property));
            _instrumentPropertySubscriptions = new Subscriptions<InstrumentPropertyValue, (object Reference, string Property), int>((i, s) => new InstrumentPropertyValue(i, s.Reference, s.Property, instrumentService));
            _currencyPropertySubscriptions = new Subscriptions<CurrencyPropertyValue, (object Reference, string Property), int>((i, s) => new CurrencyPropertyValue(i, s.Reference, s.Property, currencyService));
            _systemValueSubscriptions = new Dictionary<SystemProperty, SystemValue>();
            _portfolioComputedIds = new List<int>();

            _globalFunctions.PortfolioCalculationEnded += GlobalFunctions_PortfolioCalculationEnded;
            _portfolioListener.PortfolioChanged += PortfolioListener_PortfolioChanged;
            _positionListener.PositionChanged += PositionListener_PositionChanged;
            _transactionListener.TransactionChanged += TransactionListener_TransactionChanged;
            _instrumentStaticListener.InstrumentChanged += InstrumentStaticListener_InstrumentChanged;
            _instrumentMarketDataListener.InstrumentChanged += InstrumentMarketDataListener_InstrumentChanged;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
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

                    CSMLog.Write(_className, nameof(SubscribeToPortfolio), CSMLog.eMVerbosity.M_error, ex.ToString());
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

                    CSMLog.Write(_className, nameof(SubscribeToPosition), CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void SubscribeToFlatPosition(int portfolioId, int instrumentId, string column)
        {
            var op = _context.InvokeAsync(() => {

                _flatPositionCellSubscriptions.Add((portfolioId, instrumentId), column);
                var dp = _flatPositionCellSubscriptions.Get((portfolioId, instrumentId), column);

                try
                {
                    var da = new DataAvailableEventArgs();
                    da.FlatPositionValues.Add((portfolioId, instrumentId, column), dp.GetValue());

                    DataAvailable?.Invoke(this, da);

                    dp.Error = null;
                }
                catch (Exception ex)
                {
                    dp.Error = ex;

                    CSMLog.Write(_className, nameof(SubscribeToFlatPosition), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    CSMLog.Write(_className, nameof(SubscribeToSystemValue), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    CSMLog.Write(_className, nameof(SubscribeToPortfolioProperty), CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void SubscribeToInstrumentProperty(object id, string property)
        {
            var op = _context.InvokeAsync(() => {

                //Find the sicovam for the id
                int sicovam = 0;
                if (id is string reference)
                    sicovam = CSMInstrument.GetCode(reference);
                else
                    sicovam = Convert.ToInt32(id);

                _instrumentPropertySubscriptions.Add(sicovam, (id, property));
                var dp = _instrumentPropertySubscriptions.Get(sicovam, (id, property));

                try
                {
                    var da = new DataAvailableEventArgs();
                    da.InstrumentProperties.Add((id, property), dp.GetValue());

                    DataAvailable?.Invoke(this, da);
                }
                catch (Exception ex)
                {
                    dp.Error = ex;
                    CSMLog.Write(_className, nameof(SubscribeToInstrumentProperty), CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void SubscribeToCurrencyProperty(object id, string property)
        {
            var op = _context.InvokeAsync(() => {

                //Find the sicovam for the id
                int devise = 0;
                if (id is string reference)
                    devise = CSMCurrency.StringToCurrency(reference);
                else
                    devise = Convert.ToInt32(id);
                
                _currencyPropertySubscriptions.Add(devise, (id, property));
                var dp = _currencyPropertySubscriptions.Get(devise, (id, property));

                try
                {
                    var da = new DataAvailableEventArgs();
                    da.CurrencyProperties.Add((id, property), dp.GetValue());

                    DataAvailable?.Invoke(this, da);
                }
                catch (Exception ex)
                {
                    dp.Error = ex;
                    CSMLog.Write(_className, nameof(SubscribeToCurrencyProperty), CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void UnsubscribeFromCurrencyProperty(object id, string property)
        {
            var op = _context.InvokeAsync(() => {

                //Find the sicovam for the id
                int devise = 0;
                if (id is string reference)
                    devise = CSMCurrency.StringToCurrency(reference);
                else
                    devise = Convert.ToInt32(id);

                try
                {
                    _currencyPropertySubscriptions.Remove(devise, (id, property));
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, nameof(UnsubscribeFromCurrencyProperty), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    CSMLog.Write(_className, nameof(UnsubscribeFromPortfolio), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    CSMLog.Write(_className, nameof(UnsubscribeFromPosition), CSMLog.eMVerbosity.M_error, ex.ToString());
                }
                
            }, DispatcherPriority.Normal);
        }

        public void UnsubscribeFromFlatPosition(int portfolioId, int instrumentId, string column)
        {
            var op = _context.InvokeAsync(() => {

                try
                {
                    _flatPositionCellSubscriptions.Remove((portfolioId, instrumentId), column);
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, nameof(UnsubscribeFromFlatPosition), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    CSMLog.Write(_className, nameof(UnsubscribeFromSystemValue), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    CSMLog.Write(_className, nameof(UnsubscribeFromPortfolioProperty), CSMLog.eMVerbosity.M_error, ex.ToString());
                }

            }, DispatcherPriority.Normal);
        }

        public void UnsubscribeFromInstrumentProperty(object id, string property)
        {
            var op = _context.InvokeAsync(() => {

                //Find the sicovam for the id
                int sicovam = 0;
                if (id is string reference)
                    sicovam = CSMInstrument.GetCode(reference);
                else
                    sicovam = Convert.ToInt32(id);

                try
                {
                    _instrumentPropertySubscriptions.Remove(sicovam, (id, property));
                }
                catch (Exception ex)
                {
                    CSMLog.Write(_className, nameof(UnsubscribeFromInstrumentProperty), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                                CSMLog.Write(_className, nameof(GlobalFunctions_PortfolioCalculationEnded), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                                                CSMLog.Write(_className, nameof(GlobalFunctions_PortfolioCalculationEnded), CSMLog.eMVerbosity.M_error, ex.ToString());
                                            }

                                        }, DispatcherPriority.ApplicationIdle);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CSMLog.Write(_className, nameof(GlobalFunctions_PortfolioCalculationEnded), CSMLog.eMVerbosity.M_error, ex.ToString());
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

            void RefreshFlatPositionCells()
            {
                foreach (var cell in _flatPositionCellSubscriptions.GetCells())
                {
                    args.FlatPositionValues.Add((cell.PortfolioId, cell.InstrumentId, cell.ColumnName), cell.GetValue());
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

            RefreshFlatPositionCells();

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

        private void InstrumentStaticListener_InstrumentChanged(object sender, InstrumentChangedEventArgs e)
        {
            RefreshInstrumentStatic(e.InstrumentId, e.IsLocal);
        }

        private void InstrumentMarketDataListener_InstrumentChanged(object sender, InstrumentChangedEventArgs e)
        {
            RefreshInstrumentMarketData(e.InstrumentId, e.IsLocal);
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
                CSMLog.Write(_className, nameof(RefreshPortfolio), CSMLog.eMVerbosity.M_error, ex.ToString());
            }
        }

        private void RefreshInstrumentStatic(int instrumentId, bool isLocal)
        {
            try
            {
                void NotifyDataChanged()
                {
                    foreach (var cell in _instrumentPropertySubscriptions.GetCells().ToList())
                    {
                        if (cell.InstrumentId == instrumentId)
                        {
                            _instrumentPropertySubscriptions.Remove(cell.InstrumentId, (cell.Reference, cell.Property));
                            _instrumentPropertySubscriptions.Add(instrumentId, (cell.Reference, cell.Property));
                        }
                    }

                    var args = new DataAvailableEventArgs();

                    foreach (var value in _instrumentPropertySubscriptions.Get(instrumentId))
                    {
                        args.InstrumentProperties.Add((value.Reference, value.Property), value.GetValue());
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
                CSMLog.Write(_className, nameof(RefreshInstrumentStatic), CSMLog.eMVerbosity.M_error, ex.ToString());
            }
        }

        private void RefreshInstrumentMarketData(int instrumentId, bool isLocal)
        {
            try
            {
                void NotifyDataChanged()
                {
                    var args = new DataAvailableEventArgs();

                    foreach (var value in _instrumentPropertySubscriptions.Get(instrumentId))
                    {
                        if(value.IsMarketData())
                            args.InstrumentProperties.Add((value.Reference, value.Property), value.GetValue());
                    }

                    foreach (var value in _currencyPropertySubscriptions.Get(instrumentId))
                    {
                        if (value.IsMarketData())
                            args.CurrencyProperties.Add((value.Reference, value.Property), value.GetValue());
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
                CSMLog.Write(_className, nameof(RefreshInstrumentMarketData), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                CSMLog.Write(_className, nameof(RefreshPosition), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    CSMLog.Write(_className, nameof(RequestCalculate), CSMLog.eMVerbosity.M_error, ex.ToString());
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
                    CSMLog.Write(_className, nameof(LoadPositions), CSMLog.eMVerbosity.M_error, ex.ToString());
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
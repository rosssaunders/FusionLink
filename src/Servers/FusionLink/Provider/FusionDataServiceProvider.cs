//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using RxdSolutions.FusionLink.Interface;
using sophis.gui;
using sophis.instrument;
using sophis.market_data;
using sophis.portfolio;
using sophis.static_data;

namespace RxdSolutions.FusionLink
{
    public class FusionDataServiceProvider : IDataServerProvider
    {
        private readonly Dispatcher _context;
        private readonly IGlobalFunctions _globalFunctions;
        private readonly IPortfolioListener _portfolioListener;
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

        public FusionDataServiceProvider(IGlobalFunctions globalFunctions, IPortfolioListener portfolioListener)
        {
            _context = Dispatcher.CurrentDispatcher;
            _globalFunctions = globalFunctions;
            _portfolioListener = portfolioListener;

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
            var results = new List<int>();
            Exception ex = null;

            _context.Invoke(() => {

                using (var portfolio = CSMPortfolio.GetCSRPortfolio(folioId))
                {
                    if(portfolio is object)
                    {
                        if (!portfolio.IsLoaded())
                        {
                            ex = new PortfolioNotLoadedException();
                            return;
                        }

                        GetPositionsUI(folioId, positions, results);
                    }
                    else
                    {
                        ex = new PortfolioNotFoundException();
                        return;
                    }
                }

            });

            if (ex == null)
                return results;
            else
                throw ex;
        }

        public List<PriceHistory> GetPriceHistory(string reference, DateTime startDate, DateTime endDate)
        {
            //Find the instrumentId
            int instrumentId = 0;

            _context.Invoke(() => {

                instrumentId = CSMInstrument.GetCode(reference);

            });

            return GetPriceHistory(instrumentId, startDate, endDate);
        }

        public List<PriceHistory> GetPriceHistory(int instrumentId, DateTime startDate, DateTime endDate)
        {
            var results = new List<PriceHistory>();
            Exception ex = null;

            _context.Invoke(() => {

                using (var instrument = CSMInstrument.GetInstance(instrumentId))
                {
                    if(instrument is null)
                    {
                        ex = new InstrumentNotFoundException();
                        return;
                    }

                    int refCount = 0;
                    var history = instrument.NEW_HistoryList(DataTypeExtensions.ConvertDateTime(startDate), DataTypeExtensions.ConvertDateTime(endDate), ref refCount, null);

                    for (var i = 0; i < refCount; i++)
                    {
                        using (SSMHistory price = history.GetNthElement(i))
                        {
                            if (price.day != DataTypeExtensions.SophisNull)
                            {
                                var ph = new PriceHistory()
                                {
                                    Ask = (double?)DataTypeExtensions.ConvertDouble(price.ask, eMNullValueType.M_nvUndefined),
                                    Bid = (double?)DataTypeExtensions.ConvertDouble(price.bid, eMNullValueType.M_nvUndefined),
                                    First = (double?)DataTypeExtensions.ConvertDouble(price.first, eMNullValueType.M_nvUndefined),
                                    High = (double?)DataTypeExtensions.ConvertDouble(price.high, eMNullValueType.M_nvUndefined),
                                    Low = (double?)DataTypeExtensions.ConvertDouble(price.low, eMNullValueType.M_nvUndefined),
                                    Last = (double?)DataTypeExtensions.ConvertDouble(price.last, eMNullValueType.M_nvUndefined),
                                    Theoretical = (double?)DataTypeExtensions.ConvertDouble(price.theorical, eMNullValueType.M_nvUndefined),
                                    Volume = (double?)DataTypeExtensions.ConvertDouble(price.volume, eMNullValueType.M_nvUndefined),
                                    Date = (DateTime)price.day.GetDateTime()
                                };

                                results.Add(ph);
                            }
                        }
                    }       
                }

            });

            if (ex is null)
                return results;
            else
                throw ex;
        }

        public List<CurvePoint> GetCurvePoints(string currency, string family, string reference)
        {
            var results = new List<CurvePoint>();
            Exception ex = null;

            _context.Invoke(() => {

                var currencyCode = CSMCurrency.StringToCurrency(currency);

                if (currencyCode == 0)
                {
                    ex = new CurrencyNotFoundException();
                    return;
                }

                var familyCode = CSMYieldCurveFamily.GetYieldCurveFamilyCode(currencyCode, family);

                if(familyCode == 0)
                {
                    ex = new CurveFamilyFoundException();
                    return;
                }

                var curveId = CSMYieldCurve.LookUpYieldCurveId(familyCode, reference);

                if(curveId == 0)
                {
                    ex = new CurveNotFoundException();
                    return;
                }

                using (var yieldCurve = CSMYieldCurve.GetCSRYieldCurve(curveId))
                using (var activeCurve = yieldCurve.GetActiveSSYieldCurve())
                {
                    for(int i = 0; i < activeCurve.fPointCount; i++)
                    {
                        using (var yieldPoint = activeCurve.fPointList.GetNthElement(i))
                        {
                            var cp = new CurvePoint();
                            results.Add(cp);

                            double multiplier = 1;

                            if(yieldPoint.fType == 'x')
                            {
                                string startDateOffset = yieldPoint.fStartDate > 0 ? $"{yieldPoint.fStartDate}" : "";
                                cp.Tenor = $"{yieldPoint.fMaturity}{yieldPoint.fType}{startDateOffset}";
                                multiplier = 0.01d;
                            }
                            else
                            {
                                string startDateOffset = yieldPoint.fStartDate > 0 ? $"+{yieldPoint.fStartDate}" : "";
                                cp.Tenor = $"{yieldPoint.fMaturity}{yieldPoint.fType}{startDateOffset}";
                            }
                            
                            cp.PointType = yieldPoint.IsPointOfType(eMTypeSegment.M_etsFutureFRA) ? "FutureFRA" : yieldPoint.IsPointOfType(eMTypeSegment.M_etsMoneyMarket) ? "Money Market" : yieldPoint.IsPointOfType(eMTypeSegment.M_etsSwap) ? "Swap" : "Unknown";
                            cp.Rate = yieldPoint.fYield * multiplier;
                            cp.IsEnabled = yieldPoint.fInfoPtr.fIsUsed;
                            cp.RateCode = yieldPoint.fInfoPtr.fRateCode.ToString();
                        }
                    }
                }
            });

            if (ex is null)
                return results;
            else
                throw ex;
        }

        private void GetPositionsUI(int folioId, PositionsToRequest positions, List<int> results)
        {
            using (var portfolio = CSMPortfolio.GetCSRPortfolio(folioId))
            {
                GetPositionsFromPortfolio(portfolio, positions, results);

                var allChildren = new ArrayList();
                portfolio.GetChildren(allChildren);

                for(var i = 0; i < allChildren.Count; i++)
                {
                    var current = allChildren[i] as CSMPortfolio;

                    if(current is object)
                    {
                        GetPositionsFromPortfolio(current, positions, results);
                    }
                }
            }
        }

        private void GetPositionsFromPortfolio(CSMPortfolio portfolio, PositionsToRequest positions, List<int> results)
        {
            int positionCount = portfolio.GetTreeViewPositionCount();
            for (int i = 0; i < positionCount; i++)
            {
                using (var position = portfolio.GetNthTreeViewPosition(i))
                {
                    if (position.GetIdentifier() > 0) //Exclude Virtual FX positions
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
            }
        }

        public void SubscribeToPortfolio(int portfolioId, string column)
        {
            _context.InvokeAsync(() => {

                _portfolioCellSubscriptions.Add(portfolioId, column);

                var da = new DataAvailableEventArgs();
                da.PortfolioValues.Add((portfolioId, column), _portfolioCellSubscriptions.Get(portfolioId, column).GetValue());

                DataAvailable?.Invoke(this, da);

            }, DispatcherPriority.ApplicationIdle);
        }

        public void SubscribeToPosition(int positionId, string column)
        {
            _context.InvokeAsync(() => {

                _positionCellSubscriptions.Add(positionId, column);

                var da = new DataAvailableEventArgs();
                da.PositionValues.Add((positionId, column), _positionCellSubscriptions.Get(positionId, column).GetValue());

                DataAvailable?.Invoke(this, da);

            }, DispatcherPriority.ApplicationIdle);
        }

        public void SubscribeToSystemValue(SystemProperty property)
        {
            _context.InvokeAsync(() => {

                _systemValueSubscriptions.Add(property, GetSystemValueProperty(property));

                var da = new DataAvailableEventArgs();
                da.SystemValues.Add(property, _systemValueSubscriptions[property].GetValue());

                DataAvailable?.Invoke(this, da);

            }, DispatcherPriority.ApplicationIdle);
        }

        public void SubscribeToPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            _context.InvokeAsync(() => {

                _portfolioPropertySubscriptions.Add(portfolioId, property);

                var da = new DataAvailableEventArgs();
                da.PortfolioProperties.Add((portfolioId, property), _portfolioPropertySubscriptions.Get(portfolioId, property).GetValue());

                DataAvailable?.Invoke(this, da);

            }, DispatcherPriority.ApplicationIdle);
        }

        public void UnsubscribeToPortfolio(int portfolioId, string column)
        {
            _context.InvokeAsync(() => {

                _portfolioCellSubscriptions.Remove(portfolioId, column);

            }, DispatcherPriority.ApplicationIdle);
        }

        public void UnsubscribeToPosition(int positionId, string column)
        {
            _context.InvokeAsync(() => {

                _positionCellSubscriptions.Remove(positionId, column);

            }, DispatcherPriority.ApplicationIdle);
        }

        public void UnsubscribeToSystemValue(SystemProperty property)
        {
            _context.InvokeAsync(() => {

                _systemValueSubscriptions.Remove(property);

            }, DispatcherPriority.ApplicationIdle);
        }

        public void UnsubscribeToPortfolioProperty(int portfolioId, PortfolioProperty property)
        {
            _context.InvokeAsync(() => {

                _portfolioPropertySubscriptions.Remove(portfolioId, property);

            }, DispatcherPriority.ApplicationIdle);
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

                        ComputePortfolios(e.FolioId);

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

                                var id = e.Extraction.GetInternalID();

                                if (id == 1)
                                {
                                    _dataRefreshRequests++;
                                }

                                break;
                        }

                        //Do Nothing for now.
                        break;

                    case sophis.misc.CSMGlobalFunctions.eMPortfolioCalculationType.M_pcNotInPortfolio:
                        return;
                        
                }

                RefreshData();
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
            _context.InvokeAsync(() => 
            {
                if(e.IsLocal)
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
                
            }, DispatcherPriority.ApplicationIdle);
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
            _context.InvokeAsync(() => {

                ComputePortfolios(-1);

                RefreshData();

            }, DispatcherPriority.ApplicationIdle);
        }

        public void LoadPositions()
        {
            _context.InvokeAsync(() => {

                var portfolios = new Dictionary<int, CSMPortfolio>();
                foreach(var portfolioSubscription in _portfolioCellSubscriptions.GetCells())
                {
                    portfolios[portfolioSubscription.FolioId] = portfolioSubscription.Portfolio;
                }

                foreach(var portfolio in portfolios.Values)
                {
                    if(portfolio is object)
                    {
                        if (!portfolio.IsLoaded())
                        {
                            portfolio.Load();
                        }
                    }
                }

            }, DispatcherPriority.ApplicationIdle);

            RequestCalculate();
        }
    }
}
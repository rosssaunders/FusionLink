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
using sophisTools;

namespace RxdSolutions.FusionLink
{
    public class SophisDataServiceProvider : IDataServerProvider
    {
        private readonly SynchronizationContext _context;
        private readonly Queue<int> _portfoliosToLoad = new Queue<int>();
        private readonly Dictionary<string, CSMPortfolioColumn> _columnCache;

        private TimeSpan _lastRefreshTimer;

        public SophisDataServiceProvider(SynchronizationContext context)
        {
            _context = context;
            _lastRefreshTimer = default;
            _columnCache = new Dictionary<string, CSMPortfolioColumn>();
        }

        public bool IsBusy { get; private set; }

        public bool LoadUnloadedPortfolios { get; set; } = false;

        public TimeSpan ElapsedTimeOfLastCall => _lastRefreshTimer;

        public object GetPortfolioValue(int portfolioId, string column)
        {
            object result = "#N/A";

            _context.Send(state => {

                try
                {
                    LoadRequiredData();

                    result = GetPortfolioValueUI(portfolioId, column);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }

            }, null);

            return result;
        }

        public object GetPositionValue(int positionId, string column)
        {
            object result = "#N/A";

            _context.Send(state => {

                try
                {
                    LoadRequiredData();

                    result = GetPositionValueUI(positionId, column);
                }
                catch(Exception ex)
                {
                    result = ex.Message;
                }

            }, null);

            return result;
        }

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

                TimeUpdate(() => {

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

                });

            }, null);
        }

        public void GetPositionValues(IDictionary<(int positionId, string column), object> values)
        {
            _context.Send(state => {

                TimeUpdate(() => {

                    LoadRequiredData();

                    foreach (var key in values.Keys.ToList())
                    {
                        try
                        {
                            values[key] = GetPositionValueUI(key.positionId, key.column);
                        }
                        catch (Exception ex)
                        {
                            values[key] = ex.Message;
                        }
                    }

                });

            }, null);
        }

        public void GetPortfolioValues(IDictionary<(int positionId, string column), object> values)
        {
            _context.Send(state => {

                TimeUpdate(() => {

                    LoadRequiredData();

                    foreach (var key in values.Keys.ToList())
                    {
                        try
                        {
                            values[key] = GetPortfolioValueUI(key.positionId, key.column);
                        }
                        catch (Exception ex)
                        {
                            values[key] = ex.Message;
                        }
                    }

                });

            }, null);
        }

        public List<int> GetPositions(int folioId, Positions positions)
        {
            var results = new List<int>();

            _context.Send(state => {

                using (var portfolio = CSMPortfolio.GetCSRPortfolio(folioId))
                {
                    EnsurePortfolioLoaded(portfolio);

                    GetPositions(folioId, positions, results);
                }

            }, null);

            return results;
        }

        private void GetPositions(int folioId, Positions positions, List<int> results)
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
                        GetPositions(childPortfolio.GetCode(), positions, results);
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

        private CSMPortfolioColumn GetPortfolioColumn(string column)
        {
            if(!_columnCache.ContainsKey(column))
            {
                _columnCache.Add(column, CSMPortfolioColumn.GetCSRPortfolioColumn(column));
            }

            return _columnCache[column];
        }

        private object GetPortfolioValueUI(int portfolioId, string column)
        {
            var cv = new SSMCellValue();

            using (var portfolio = CSMPortfolio.GetCSRPortfolio(portfolioId))
            using (var cs = new SSMCellStyle())
            {
                if (portfolio is null)
                {
                    return $"The requested Portfolio '{portfolioId}' cannot be found";
                }

                var portfolioColumn = GetPortfolioColumn(column);

                if (portfolioColumn is null)
                {
                    return $"The requested Portfolio Column '{column}' cannot be found";
                }

                if (!portfolio.IsLoaded())
                {
                    if (LoadUnloadedPortfolios)
                    {
                        _portfoliosToLoad.Enqueue(portfolioId);
                        return "Loading the portfolio (F8)... please wait";
                    }
                    else
                    {
                        return $"The requested portfolio '{portfolioId}' is not loaded. Please load in the FusionInvest client.";
                    }
                }
                else
                {
                    portfolioColumn.GetPortfolioCell(portfolioId, portfolioId, null, ref cv, cs, true);

                    return ExtractValueFromSophisCell(cv, cs);
                }
            }
        }

        private object GetPositionValueUI(int positionId, string column)
        {
            var cv = new SSMCellValue();

            using (var position = CSMPosition.GetCSRPosition(positionId))
            using (var cs = new SSMCellStyle())
            {
                if (position is null)
                {
                    return $"The requested position '{positionId}' is either not loaded or does not exist. Please load the positions portfolio in Sophis.";
                }

                var portfolioColumn = GetPortfolioColumn(column);
                if (portfolioColumn is null)
                {
                    return $"The requested Portfolio Column '{column}' cannot be found";
                }

                using (var portfolio = position.GetPortfolio())
                {
                    if (!portfolio.IsLoaded())
                    {
                        if (LoadUnloadedPortfolios)
                        {
                            _portfoliosToLoad.Enqueue(portfolio.GetCode());
                            return "Loading the portfolio. Please wait...";
                        }
                        else
                        {
                            return $"The requested portfolio '{portfolio.GetCode()}' is not loaded. Please load in the FusionInvest client.";
                        }
                    }
                    else
                    {
                        portfolioColumn.GetPositionCell(position, position.GetPortfolioCode(), position.GetPortfolioCode(), null, 0, position.GetInstrumentCode(), ref cv, cs, true);

                        return ExtractValueFromSophisCell(cv, cs);
                    }
                }
            }
        }

        private object ExtractValueFromSophisCell(SSMCellValue cv, SSMCellStyle cs)
        {
            switch (cs.kind)
            {
                case NSREnums.eMDataType.M_dDate:
                case NSREnums.eMDataType.M_dDateTime:
                    {
                        var day = new CSMDay(cv.integerValue);
                        return new DateTime(day.fYear, day.fMonth, day.fDay);
                    }

                case NSREnums.eMDataType.M_dInt:
                    return (long)cv.integerValue;

#if !V72
                case NSREnums.eMDataType.M_dPascalString:
#endif
                case NSREnums.eMDataType.M_dUnicodeString:
                case NSREnums.eMDataType.M_dNullTerminatedString:
                    return cv.GetString();

                case NSREnums.eMDataType.M_dLong:
                case NSREnums.eMDataType.M_dLongLong:
                    return cv.longlongValue;

                case NSREnums.eMDataType.M_dArray:
                    return cv.GetString();

                case NSREnums.eMDataType.M_dSlidingDate:
                    return cv.GetString();

                case NSREnums.eMDataType.M_dBool:
                    return cv.shortInteger;

                case NSREnums.eMDataType.M_dPointer:
                    return cv.shortInteger;

                case NSREnums.eMDataType.M_dDouble:
                    return cv.doubleValue;

                case NSREnums.eMDataType.M_dFloat:
                    return (double)cv.floatValue;

                case NSREnums.eMDataType.M_dShort:
                    return (double)cv.shortInteger;

                case NSREnums.eMDataType.M_dSmallIcon:
                    return (double)cv.iconIdentifier;
            }

            throw new ApplicationException("Unknown eMDataType");
        }

        private void LoadRequiredData()
        {
            if (LoadUnloadedPortfolios)
            {
                if (_portfoliosToLoad.Count > 0)
                {
                    while (_portfoliosToLoad.Count > 0)
                    {
                        int portfolioId = _portfoliosToLoad.Dequeue();
                        using (var portfolio = CSMPortfolio.GetCSRPortfolio(portfolioId))
                        {
                            EnsurePortfolioLoaded(portfolio);
                        }
                    }
                }
            }
        }

        private void EnsurePortfolioLoaded(CSMPortfolio portfolio)
        {
            if (!portfolio.IsLoaded())
            {
                IsBusy = true;

                portfolio.Load();
                portfolio.Compute();

                IsBusy = false;
            }
        }

        private void TimeUpdate(Action action)
        {
            var timer = Stopwatch.StartNew();

            action.Invoke();

            timer.Stop();

            _lastRefreshTimer = timer.Elapsed;
        }
    }
}
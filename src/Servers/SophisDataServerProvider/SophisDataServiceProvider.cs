//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Threading;
using RxdSolutions.FusionLink.Interface;
using sophis.portfolio;
using sophisTools;

namespace RxdSolutions.FusionLink
{
    public class SophisDataServiceProvider : IDataServerProvider
    {
        private readonly SynchronizationContext _context;

        private Queue<int> _portfoliosToLoad = new Queue<int>();

        public SophisDataServiceProvider(SynchronizationContext context)
        {
            _context = context;
        }

        public bool IsBusy { get; private set; }

        public bool LoadUnloadedPortfolios { get; set; } = false;

        private void LoadRequiredData()
        {
            if(LoadUnloadedPortfolios)
            {
                if (_portfoliosToLoad.Count > 0)
                {
                    while (_portfoliosToLoad.Count > 0)
                    {
                        var portfolioId = _portfoliosToLoad.Dequeue();
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

        public object GetPortfolioValue(int portfolioId, string column)
        {
            object result = "#N/A";

            _context.Send(state => {

                try
                {
                    LoadRequiredData();

                    var cv = new SSMCellValue();

                    using (var portfolio = CSMPortfolio.GetCSRPortfolio(portfolioId))
                    using (var portfolioColumn = CSMPortfolioColumn.GetCSRPortfolioColumn(column))
                    using (var cs = new SSMCellStyle())
                    {
                        if(portfolio is null)
                        {
                            result = $"The requested Portfolio '{portfolioId}' cannot be found";
                            return;
                        }

                        if(portfolioColumn is null)
                        {
                            result = $"The requested Portfolio Column '{column}' cannot be found";
                            return;
                        }

                        if (!portfolio.IsLoaded())
                        {
                            if(LoadUnloadedPortfolios)
                            {
                                result = "Loading the portfolio (F8)... please wait";
                                _portfoliosToLoad.Enqueue(portfolioId);
                            }
                            else
                            {
                                result = $"The requested portfolio '{portfolioId}' is not loaded. Please load in the FusionInvest client.";
                            }
                        }
                        else
                        {
                            portfolioColumn.GetPortfolioCell(portfolioId, portfolioId, null, ref cv, cs, true);

                            result = GetDataPoint(column, portfolioId, cv, cs);
                        }
                    }
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

                    var cv = new SSMCellValue();

                    using (var position = CSMPosition.GetCSRPosition(positionId))
                    using (var portfolioColumn = CSMPortfolioColumn.GetCSRPortfolioColumn(column))
                    using (var cs = new SSMCellStyle())
                    {
                        if (position is null)
                        {
                            result = $"The requested position '{positionId}' is either not loaded or does not exist. Please load the positions portfolio in Sophis.";
                            return;
                        }

                        if (portfolioColumn is null)
                        {
                            result = $"The requested Portfolio Column '{column}' cannot be found";
                            return;
                        }

                        using (var portfolio = position.GetPortfolio())
                        {
                            if (!portfolio.IsLoaded())
                            {
                                if(LoadUnloadedPortfolios)
                                {
                                    result = "Loading the portfolio. Please wait...";

                                    _portfoliosToLoad.Enqueue(portfolio.GetCode());
                                }
                                else
                                {
                                    result = $"The requested portfolio '{portfolio.GetCode()}' is not loaded. Please load in the FusionInvest client.";
                                }
                            }
                            else
                            {
                                portfolioColumn.GetPositionCell(position, position.GetPortfolioCode(), position.GetPortfolioCode(), null, 0, position.GetInstrumentCode(), ref cv, cs, true);

                                result = GetDataPoint(column, positionId, cv, cs);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    result = ex.Message;
                }

            }, null);

            return result;
        }

        private object GetDataPoint(string column, int position, SSMCellValue cv, SSMCellStyle cs)
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

                case NSREnums.eMDataType.M_dPascalString:
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

        public DateTime GetPortfolioDate()
        {
            DateTime ? dt = null;

            _context.Send(state => {

                var portfolioDate = CSMPortfolio.GetPortfolioDate();
                using (var day = new CSMDay(portfolioDate))
                {
                    dt = new DateTime(day.fYear, day.fMonth, day.fDay);
                }

            }, null);

            if(dt.HasValue)
                return dt.Value;

            throw new ApplicationException("Unable to get the Portfolio date");
        }

        public List<int> GetPositions(int folioId)
        {
            var results = new List<int>();

            _context.Send(state => {

                using (var portfolio = CSMPortfolio.GetCSRPortfolio(folioId))
                {
                    EnsurePortfolioLoaded(portfolio);

                    GetPositions(folioId, results);
                }

            }, null);

            return results;
        }

        private void GetPositions(int folioId, List<int> positions)
        {
            using (var portfolio = CSMPortfolio.GetCSRPortfolio(folioId))
            {
                var positionCount = portfolio.GetTreeViewPositionCount();
                for (var i = 0; i < positionCount; i++)
                {
                    using (var position = portfolio.GetNthTreeViewPosition(i))
                    {
                        positions.Add(position.GetIdentifier());
                    }
                }

                var childPortfolioCount = portfolio.GetChildCount();

                for(var i = 0; i < childPortfolioCount; i++)
                {
                    using (var childPortfolio = portfolio.GetNthChild(i))
                    {
                        GetPositions(childPortfolio.GetCode(), positions);
                    }
                }
            }
        }
    }
}
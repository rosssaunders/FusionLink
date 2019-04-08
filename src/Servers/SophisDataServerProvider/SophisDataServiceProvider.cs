//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Threading;
using RxdSolutions.Sophis2Excel.Interface;
using sophis.portfolio;

namespace RxdSolutions.Sophis2Excel
{
    public class SophisDataServiceProvider : IDataServiceProvider
    {
        private readonly SynchronizationContext _context;

        private Queue<int> _portfoliosToLoad = new Queue<int>();

        private Queue<int> _positionsToLoad = new Queue<int>();

        public SophisDataServiceProvider(SynchronizationContext context)
        {
            this._context = context;
        }

        public bool IsBusy { get; private set; }

        private void LoadRequiredData()
        {
            if(_portfoliosToLoad.Count > 0)
            {
                IsBusy = true;

                while (_portfoliosToLoad.Count > 0)
                {
                    var portfolioId = _portfoliosToLoad.Dequeue();
                    using (var portfolio = CSMPortfolio.GetCSRPortfolio(portfolioId))
                    {
                        if(!portfolio.IsLoaded())
                        {
                            portfolio.Load();
                            portfolio.Compute();
                        }
                    }   
                }
                
                IsBusy = false;
            }

            if(_positionsToLoad.Count > 0)
            {
                IsBusy = true;

                while (_positionsToLoad.Count > 0)
                {
                    var positionId = _positionsToLoad.Dequeue();
                    using (var position  = CSMPosition.GetCSRPosition(positionId))
                    using (var portfolio = CSMPortfolio.GetCSRPortfolio(position.GetPortfolioCode()))
                    {
                        if(!portfolio.IsLoaded())
                        {
                            portfolio.Load();
                            portfolio.Compute();
                        }
                    }
                }

                IsBusy = false;
            }
        }

        public (DataTypeEnum dataType, object value) GetPortfolioValue(int portfolioId, string column)
        {
            (DataTypeEnum dataType, object value) result = (DataTypeEnum.String, "#N/A");

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
                            result = (DataTypeEnum.String, $"The requested Portfolio '{portfolioId}' cannot be found");
                            return;
                        }

                        if(portfolioColumn is null)
                        {
                            result = (DataTypeEnum.String, $"The requested Portfolio Column '{column}' cannot be found");
                            return;
                        }

                        if (!portfolio.IsLoaded())
                        {
                            result = (DataTypeEnum.String, "Loading the portfolio (F8)... please wait");

                            _portfoliosToLoad.Enqueue(portfolioId);
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
                    result = (DataTypeEnum.String, ex.Message);
                }

            }, null);

            return result;
        }

        public (DataTypeEnum dataType, object value) GetPositionValue(int positionId, string column)
        {
            (DataTypeEnum dataType, object value) result = (DataTypeEnum.String, "#N/A");

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
                            result = (DataTypeEnum.String, $"The requested Position '{positionId}' cannot be found");
                            return;
                        }

                        if (portfolioColumn is null)
                        {
                            result = (DataTypeEnum.String, $"The requested Portfolio Column '{column}' cannot be found");
                            return;
                        }

                        using (var portfolio = position.GetPortfolio())
                        {
                            if (!portfolio.IsLoaded())
                            {
                                result = (DataTypeEnum.String, "Loading the position");

                                _positionsToLoad.Enqueue(positionId);
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
                    result = (DataTypeEnum.String, ex.Message);
                }

            }, null);

            return result;
        }

        private (DataTypeEnum type, object value) GetDataPoint(string column, int position, SSMCellValue cv, SSMCellStyle cs)
        {
            switch (cs.kind)
            {
                case NSREnums.eMDataType.M_dDate:
                case NSREnums.eMDataType.M_dDateTime:
                    return (DataTypeEnum.DateTime, cv.GetString());

                case NSREnums.eMDataType.M_dInt:
                    return (DataTypeEnum.Int64, (long)cv.integerValue);

                case NSREnums.eMDataType.M_dPascalString:
                case NSREnums.eMDataType.M_dUnicodeString:
                case NSREnums.eMDataType.M_dNullTerminatedString:
                    return (DataTypeEnum.String, cv.GetString());

                case NSREnums.eMDataType.M_dLong:
                case NSREnums.eMDataType.M_dLongLong:
                    return (DataTypeEnum.Int64, cv.longlongValue);

                case NSREnums.eMDataType.M_dArray:
                    return (DataTypeEnum.String, cv.GetString());

                case NSREnums.eMDataType.M_dSlidingDate:
                    return (DataTypeEnum.String, cv.GetString());

                case NSREnums.eMDataType.M_dBool:
                    return (DataTypeEnum.Boolean, cv.shortInteger);

                case NSREnums.eMDataType.M_dPointer:
                    return (DataTypeEnum.Int64, cv.shortInteger);

                case NSREnums.eMDataType.M_dDouble:
                    return (DataTypeEnum.Double, cv.doubleValue);

                case NSREnums.eMDataType.M_dFloat:
                    return (DataTypeEnum.Double, (double)cv.floatValue);

                case NSREnums.eMDataType.M_dShort:
                    return (DataTypeEnum.Double, (double)cv.shortInteger);

                case NSREnums.eMDataType.M_dSmallIcon:
                    return (DataTypeEnum.Double, (double)cv.iconIdentifier);
            }

            throw new ApplicationException("Unknown eMDataType");
        }
    }
}
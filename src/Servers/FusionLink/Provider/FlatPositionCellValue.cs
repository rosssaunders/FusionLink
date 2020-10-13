//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Runtime.ExceptionServices;
using RxdSolutions.FusionLink.Helpers;
using RxdSolutions.FusionLink.Properties;
using sophis.portfolio;

namespace RxdSolutions.FusionLink.Provider
{
    internal class FlatPositionCellValue : CellValueBase
    {
        public CSMPortfolio Portfolio { get; private set; }

        public int PortfolioId { get; }

        public int InstrumentId { get; }

        public FlatPositionCellValue(int portfolioId, int instrumentId, string column, CSMExtraction extraction) : base(column, extraction)
        {
            PortfolioId = portfolioId;
            InstrumentId = instrumentId;
            Portfolio = CSMPortfolio.GetCSRPortfolio(portfolioId);
        }

        [HandleProcessCorruptedStateExceptions]
        public override object GetValue()
        {
            object GetValueInternal()
            {
                if (Column is null)
                {
                    return string.Format(Resources.ColumnNotFoundMessage, ColumnName);
                }

                var position = FindFlatPosition();

                if (position is object)
                {
                    Column.GetPositionCell(position, PortfolioId, PortfolioId, Portfolio.GetExtraction(), 0, InstrumentId, ref CellValue, CellStyle, false);

                    var value = CellValue.ExtractValueFromSophisCell(CellStyle);

                    return value;
                }
                else
                {
                    return string.Format(Resources.FlatPositionNotLoadedOrMissingMessage, PortfolioId, InstrumentId);
                }
            }

            try
            {
                return GetValueInternal();
            }
            catch(Exception e)
            {
                try
                {
                    var errorMessage = e.Message;

                    Portfolio?.Dispose();
                    Column?.Dispose();

                    Column = CSMPortfolioColumn.GetCSRPortfolioColumn(ColumnName);
                    Portfolio = CSMPortfolio.GetCSRPortfolio(PortfolioId);

                    var position = FindFlatPosition();

                    return GetValueInternal();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        private CSMPosition FindFlatPosition()
        {
            for (var i = 0; i < Portfolio.GetFlatViewPositionCount(); i++)
            {
                var currentPosition = Portfolio.GetNthFlatViewPosition(i);
                if (currentPosition.GetInstrumentCode() == InstrumentId)
                    return currentPosition;
            }

            return null;
        }

        private bool disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Portfolio?.Dispose();
                }

                disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}
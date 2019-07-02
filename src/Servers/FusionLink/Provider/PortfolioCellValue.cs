//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Runtime.ExceptionServices;
using RxdSolutions.FusionLink.Helpers;
using RxdSolutions.FusionLink.Properties;
using sophis.portfolio;

namespace RxdSolutions.FusionLink.Provider
{
    internal class PortfolioCellValue : CellValueBase
    {
        public CSMPortfolio Portfolio { get; private set; }

        public int FolioId { get; }

        public PortfolioCellValue(int folioId, string column, CSMExtraction extraction) : base(column, extraction)
        {
            FolioId = folioId;
            Portfolio = CSMPortfolio.GetCSRPortfolio(folioId);
        }

        [HandleProcessCorruptedStateExceptions]
        public override object GetValue()
        {
            object GetValueInternal()
            {
                if (Error is object)
                {
                    return Error.Message;
                }

                if (Portfolio is null)
                {
                    return string.Format(Resources.PortfolioNotFoundMessage, FolioId);
                }

                if (!Portfolio.IsLoaded())
                {
                    return string.Format(Resources.PortfolioNotLoadedMessage, FolioId);
                }

                if (Column is null)
                {
                    return string.Format(Resources.ColumnNotFoundMessage, ColumnName);
                }

                Column.GetPortfolioCell(Portfolio.GetCode(), Portfolio.GetCode(), Extraction, ref CellValue, CellStyle, false);

                var value = CellValue.ExtractValueFromSophisCell(CellStyle);

                return value;
            }

            try
            {
                return GetValueInternal();
            }
            catch
            {
                Portfolio?.Dispose();
                Portfolio = CSMPortfolio.GetCSRPortfolio(FolioId);

                Column?.Dispose();
                Column = CSMPortfolioColumn.GetCSRPortfolioColumn(ColumnName);

                try
                {
                    return GetValueInternal();
                }
                catch(Exception ex)
                {
                    return ex.Message;
                }
            }
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
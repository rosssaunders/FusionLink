//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using RxdSolutions.FusionLink.Properties;
using sophis.portfolio;
using sophisTools;

namespace RxdSolutions.FusionLink
{
    internal class PortfolioCellValue : CellValueBase
    {
        public CSMPortfolio Portfolio { get; }

        public int FolioId { get; }

        public PortfolioCellValue(int folioId, string column) : base(column)
        {
            FolioId = folioId;
            Portfolio = CSMPortfolio.GetCSRPortfolio(folioId);
        }

        public override object GetValue()
        {
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

            Column.GetPortfolioCell(Portfolio.GetCode(), Portfolio.GetCode(), null, ref CellValue, CellStyle, true);

            return CellValue.ExtractValueFromSophisCell(CellStyle);
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
//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using sophis.portfolio;

namespace RxdSolutions.FusionLink.Provider
{
    internal abstract class CellValueBase : IDisposable
    {
        public SSMCellValue CellValue = new SSMCellValue();

        public SSMCellStyle CellStyle = new SSMCellStyle();

        public CSMExtraction Extraction { get; }

        public CSMPortfolioColumn Column { get; protected set; }

        public string ColumnName { get; }

        public Exception Error { get; set; }

        protected CellValueBase(string columnName, CSMExtraction extraction)
        {
            ColumnName = columnName;

            Column = CSMPortfolioColumn.GetCSRPortfolioColumn(columnName);

            Extraction = extraction;
        }

        public abstract object GetValue();

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Column?.Dispose();
                    CellStyle?.Dispose();
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
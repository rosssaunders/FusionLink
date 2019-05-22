//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    internal abstract class CellValueBase
    {
        public SSMCellValue CellValue = new SSMCellValue();

        public SSMCellStyle CellStyle = new SSMCellStyle();

        public CSMPortfolioColumn Column { get; }

        public string ColumnName { get; }

        protected CellValueBase(string columnName)
        {
            ColumnName = columnName;

            Column = CSMPortfolioColumn.GetCSRPortfolioColumn(columnName);
        }

        public abstract object GetValue();
    }
}
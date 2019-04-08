//  Copyright (c) RXD Solutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using RxdSolutions.Sophis2Excel.Interface;

namespace RTD.Excel
{
    public class PortfolioValueReceivedEventArgs : ValueSentEventArgs
    {
        public int PortfolioId { get; private set; }

        public PortfolioValueReceivedEventArgs(int portfolioId, string column, DataTypeEnum dataType, object value)
            : base(column, dataType, value)
        {
            this.PortfolioId = portfolioId;
        }
    }
}
//  Copyright (c) RXD SOlutions. All rights reserved.
//  Sophis2Excel is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.Sophis2Excel.Interface;

namespace RxdSolutions
{
    public interface IDataServiceProvider
    {
        bool IsBusy { get; }

        (DataTypeEnum dataType, object value) GetPositionValue(int positionId, string column);

        (DataTypeEnum dataType, object value) GetPortfolioValue(int portfolioId, string column);
    }
}

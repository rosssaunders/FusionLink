//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public interface IDataServerProvider
    {
        bool IsBusy { get; }

        object GetPositionValue(int positionId, string column);

        object GetPortfolioValue(int portfolioId, string column);

        DateTime GetPortfolioDate();

        List<int> GetPositions(int folioId);
    }
}

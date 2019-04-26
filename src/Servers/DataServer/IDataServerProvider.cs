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

        TimeSpan ElapsedTimeOfLastCall { get; }

        object GetPositionValue(int positionId, string column);

        void GetPositionValues(IDictionary<(int positionId, string column), object> values);

        object GetPortfolioValue(int portfolioId, string column);

        void GetPortfolioValues(IDictionary<(int positionId, string column), object> values);

        object GetSystemValue(SystemProperty property);

        void GetSystemValues(IDictionary<SystemProperty, object> values);

        List<int> GetPositions(int folioId);
    }
}

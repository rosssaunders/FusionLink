//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink
{
    public class DataAvailableEventArgs : EventArgs
    {
        public DataAvailableEventArgs()
        {
            PortfolioValues = new Dictionary<(int, string), object>();
            PositionValues = new Dictionary<(int, string), object>();
            SystemValues = new Dictionary<SystemProperty, object>();
        }

        public IDictionary<(int folioId, string column), object> PortfolioValues { get; }

        public IDictionary<(int positionId, string column), object> PositionValues { get; }

        public IDictionary<SystemProperty, object> SystemValues { get; }
    }
}

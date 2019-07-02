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
            PortfolioProperties = new Dictionary<(int, PortfolioProperty), object>();
            PositionValues = new Dictionary<(int, string), object>();
            SystemValues = new Dictionary<SystemProperty, object>();
        }

        public TimeSpan TimeTaken { get; set; }

        public IDictionary<(int folioId, string column), object> PortfolioValues { get; }

        public IDictionary<(int positionId, string column), object> PositionValues { get; }

        public IDictionary<(int positionId, PortfolioProperty property), object> PortfolioProperties { get; }

        public IDictionary<SystemProperty, object> SystemValues { get; }
    }
}

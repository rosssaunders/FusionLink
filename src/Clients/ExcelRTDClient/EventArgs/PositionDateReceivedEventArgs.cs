//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class PortfolioDateReceivedEventArgs : EventArgs
    {
        public DateTime PortfolioDate { get; private set; }

        public PortfolioDateReceivedEventArgs(DateTime date)
        {
            this.PortfolioDate = date;
        }
    }
}
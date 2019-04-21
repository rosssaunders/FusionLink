//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.RTDClient
{
    public class PortfolioValueReceivedEventArgs : ValueSentEventArgs
    {
        public int PortfolioId { get; private set; }

        public PortfolioValueReceivedEventArgs(int portfolioId, string column, object value)
            : base(column, value)
        {
            this.PortfolioId = portfolioId;
        }
    }
}
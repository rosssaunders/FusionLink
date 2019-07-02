//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

namespace RxdSolutions.FusionLink.Listeners
{
    public class PortfolioChangedEventArgs
    {
        public int PortfolioId { get; }

        public bool IsLocal { get; }

        public PortfolioChangedEventArgs(int portfolioId, bool isLocal)
        {
            PortfolioId = portfolioId;
            IsLocal = isLocal;
        }
    }
}
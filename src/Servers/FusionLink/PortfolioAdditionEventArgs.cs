//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    public class PortfolioAdditionEndedEventArgs : EventArgs
    {
        public CSMExtraction Extraction { get; }

        public int FolioId { get; }

        public int PortfolioRefreshVersion { get; }

        public PortfolioAdditionEndedEventArgs(CSMExtraction extraction, int folioId, int portfolioRefreshVersion)
        {
            Extraction = extraction;
            FolioId = folioId;
            PortfolioRefreshVersion = portfolioRefreshVersion;
        }
    }
}
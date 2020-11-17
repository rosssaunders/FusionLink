//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    public class PortfolioAdditionEndedEventArgs : EventArgs
    {
        public CSMExtraction Extraction { get; }

        public int FolioId { get; }

        public int PortfolioRefreshVersion { get; }

        public bool LoadingPortfolio { get; }

        public PortfolioAdditionEndedEventArgs(CSMExtraction extraction, int folioId, int portfolioRefreshVersion, bool fLoadingPortfolio)
        {
            Extraction = extraction;
            FolioId = folioId;
            PortfolioRefreshVersion = portfolioRefreshVersion;
            LoadingPortfolio = fLoadingPortfolio;
        }
    }
}
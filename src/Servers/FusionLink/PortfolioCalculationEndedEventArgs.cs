//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    public class PortfolioCalculationEndedEventArgs : EventArgs
    {
        public CSMExtraction Extraction { get; }

        public int FolioId { get; }

        public int PortfolioRefreshVersion { get; }

        public sophis.misc.CSMGlobalFunctions.eMPortfolioCalculationType InPortfolioCalculation { get; }

        public bool LoadingPortfolio { get; }

        public PortfolioCalculationEndedEventArgs(CSMExtraction extraction, int folioId, int portfolioRefreshVersion, sophis.misc.CSMGlobalFunctions.eMPortfolioCalculationType fInPortfolioCalculation, bool fLoadingPortfolio)
        {
            Extraction = extraction;
            FolioId = folioId;
            PortfolioRefreshVersion = portfolioRefreshVersion;
            InPortfolioCalculation = fInPortfolioCalculation;
            LoadingPortfolio = fLoadingPortfolio;
        }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections;
using System.Diagnostics;
using sophis.instrument;
using sophis.misc;
using sophis.portfolio;
using sophis.utils;
using sophis.value;

namespace RxdSolutions.FusionLink
{
    public class SophisRisqueGlobalFunctions : CSMGlobalFunctions, IGlobalFunctions
    {
        public event EventHandler<PortfolioCalculationEndedEventArgs> PortfolioCalculationEnded;

#if !V72

        public override void EndPortfolioCalculation(CSMExtraction extraction, int folioId)
        {
            PortfolioCalculationEnded?.Invoke(this, new PortfolioCalculationEndedEventArgs(extraction, folioId, CSMPortfolioColumn.GetRefreshVersion(), fInPortfolioCalculation, m_Mode, this.fLoadingPortfolio));

            base.EndPortfolioCalculation(extraction, folioId);
        }

#endif

#if V72

        public override void EndPortfolioCalculation(CSMExtraction extraction, int folio_id, bool full, ArrayList impactedPortfolios)
        {
            PortfolioCalculationEnded?.Invoke(this, new PortfolioCalculationEndedEventArgs(extraction, folio_id, CSMPortfolioColumn.GetRefreshVersion(), fInPortfolioCalculation, m_Mode, this.fLoadingPortfolio));

            base.EndPortfolioCalculation(extraction, folio_id, full, impactedPortfolios);
        }

#endif
    }

    public class FusionInvestGlobalFunctions : CSMAmGlobalFunctions, IGlobalFunctions
    {
        public event EventHandler<PortfolioCalculationEndedEventArgs> PortfolioCalculationEnded;

#if !V72

        public override void EndPortfolioCalculation(CSMExtraction extraction, int folioId)
        {
            PortfolioCalculationEnded?.Invoke(this, new PortfolioCalculationEndedEventArgs(extraction, folioId, CSMPortfolioColumn.GetRefreshVersion(), fInPortfolioCalculation, m_Mode, this.fLoadingPortfolio));

            base.EndPortfolioCalculation(extraction, folioId);
        }

#endif

#if V72

        public override void EndPortfolioCalculation(CSMExtraction extraction, int folio_id, bool full, ArrayList impactedPortfolios)
        {
            PortfolioCalculationEnded?.Invoke(this, new PortfolioCalculationEndedEventArgs(extraction, folio_id, CSMPortfolioColumn.GetRefreshVersion(), fInPortfolioCalculation, m_Mode, this.fLoadingPortfolio));

            base.EndPortfolioCalculation(extraction, folio_id, full, impactedPortfolios);
        }

#endif
    }
}
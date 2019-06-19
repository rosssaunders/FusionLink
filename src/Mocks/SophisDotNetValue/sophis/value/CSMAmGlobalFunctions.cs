//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Collections;
using sophis.misc;
using sophis.portfolio;

namespace sophis.value
{
    public class CSMAmGlobalFunctions : CSMGlobalFunctions
    {
        public unsafe CSMAmGlobalFunctions() : base(null, null)
        {
        }


#if !V72

        public unsafe virtual void EndPortfolioCalculation(CSMExtraction extraction, int folio_id)
        {
        }

#endif

#if V72

        public unsafe virtual void EndPortfolioCalculation(CSMExtraction extraction, int folio_id, bool full, ArrayList impactedPortfolios)
        {
        }

#endif
    }
}
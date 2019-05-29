//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.misc;
using sophis.portfolio;

namespace sophis.value
{
    public class CSMAmGlobalFunctions : CSMGlobalFunctions
    {
        public unsafe CSMAmGlobalFunctions() : base(null, null)
        {
        }

        public unsafe override void EndPortfolioCalculation(CSMExtraction extraction, int folioId)
        {
            throw new NotImplementedException();
        }

        public unsafe override void EndPortfolioAddition(CSMExtraction extraction, int folioId)
        {
            throw new NotImplementedException();
        }
    }
}
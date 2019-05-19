//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.portfolio;

namespace sophis.misc
{
    public class CSMGlobalFunctions : IDisposable
    {
        public unsafe static void Register(CSMGlobalFunctions ptr)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void EndPortfolioAddition(CSMExtraction extraction, int folio_id)
        {
        }

        public unsafe virtual void EndPortfolioCalculation()
        {
        }
    }
}
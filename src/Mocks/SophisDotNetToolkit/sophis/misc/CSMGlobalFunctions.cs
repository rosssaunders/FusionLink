//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using MEnums;
using sophis.portfolio;

namespace sophis.misc
{
    public class CSMGlobalFunctions : IDisposable
    {
        public unsafe CSMGlobalFunctions()
        {
        }

        protected unsafe CSMGlobalFunctions(void* A_1, void* A_2)
        {
        }

        protected eMode m_Mode;

#pragma warning disable IDE1006 // Naming Styles
        public unsafe bool fLoadingPortfolio { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public enum eMPortfolioCalculationType
        {
            M_pcJustSumming = 2,
            M_pcFullCalculation = 1,
            M_pcNotInPortfolio = 0
        }

#pragma warning disable IDE1006 // Naming Styles
        public unsafe eMPortfolioCalculationType fInPortfolioCalculation { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public unsafe eMPortfolioCalculationType IsInPortfolioCalculation()
        {
            throw new NotImplementedException();
        }

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

        public unsafe virtual void EndPortfolioCalculation(CSMExtraction extraction, int folio_id)
        {
        }

        public unsafe static CSMGlobalFunctions GetCurrentGlobalFunctions()
        {
            throw new NotImplementedException();
        }
    }
}
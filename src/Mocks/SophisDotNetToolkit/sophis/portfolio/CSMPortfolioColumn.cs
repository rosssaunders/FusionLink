//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.instrument;
using sophis.utils;

namespace sophis.portfolio
{
    public unsafe class CSMPortfolioColumn : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public unsafe static CSMPortfolioColumn GetCSRPortfolioColumn(CMString name)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void GetPositionCell(int activePortfolioCode, int portfolioCode, CSMExtraction extraction, int underlyingCode, int instrumentCode, eMPositionType positionType, int positionIdentifier, ref SSMCellValue cellValue, SSMCellStyle cellStyle, bool onlyTheValue)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void GetPositionCell(CSMPosition position, int activePortfolioCode, int portfolioCode, CSMExtraction extraction, int underlyingCode, int instrumentCode, ref SSMCellValue cellValue, SSMCellStyle cellStyle, bool onlyTheValue)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void GetPortfolioCell(int activePortfolioCode, int portfolioCode, CSMExtraction extraction, ref SSMCellValue cellValue, SSMCellStyle cellStyle, bool onlyTheValue)
        {
            throw new NotImplementedException();
        }
    }
}
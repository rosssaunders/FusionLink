//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.tools;

namespace sophis.portfolio
{
    public class CSMPortfolioAction : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public enum eMOrder
        {
            M_oAfter = 2,
            M_oSave = 1,
            M_oBefore = 0
        }

        public static void Register(string key, eMOrder e, CSMPortfolioAction prototype)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void NotifyCreated(CSMPortfolio portfolio, CSMEventVector message)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void NotifyModified(CSMPortfolio portfolio, CSMEventVector message)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void NotifyTransferred(CSMPortfolio portfolio, CSMEventVector message)
        {
            throw new NotImplementedException();
        }

        public unsafe virtual void NotifyDeleted(CSMPortfolio portfolio, CSMEventVector message)
        {
            throw new NotImplementedException();
        }
    }
}

//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace sophis.market_data
{
    public class CSMInfoSup : IDisposable
    {
        public unsafe bool fIsUsed { get; set; }

        public unsafe int fRateCode { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

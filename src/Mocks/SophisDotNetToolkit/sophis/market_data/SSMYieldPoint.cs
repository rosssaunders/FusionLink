//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace sophis.market_data
{
    public class SSMYieldPoint : IDisposable
    {
        public CSMInfoSup fInfoPtr { get; set; }

        public double fYield { get; set; }

        public unsafe short fStartDate { get; set; }

        public char fType { get; set; }

        public int fMaturity { get; set; }

        public bool IsPointOfType(eMTypeSegment typeSegment)
        {
            throw new NotImplementedException();
        }

        public SSMYieldPoint GetNthElement(int nIndex)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

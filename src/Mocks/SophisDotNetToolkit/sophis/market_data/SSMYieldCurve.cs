//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace sophis.market_data
{
    public class SSMYieldCurve : IDisposable
    {

#if V72

        public unsafe sophis.DAL.MYieldCurvePoints fPoints { get; set; }

#else
        public unsafe int fPointCount { get; set; }

        public unsafe SSMYieldPoint fPointList { get; set; }

#endif

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

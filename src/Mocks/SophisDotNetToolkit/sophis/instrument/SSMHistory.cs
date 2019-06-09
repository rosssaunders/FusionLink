//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace sophis.instrument
{
    public class SSMHistory : IDisposable
    {
        public double ask { get; set; }

        public double bid { get; set; }

        public double theorical { get; set; }

        public double volume { get; set; }

        public double last { get; set; }

        public double low { get; set; }

        public double high { get; set; }

        public double first { get; set; }

        public int day { get; set; }

        public SSMHistory GetNthElement(int nIndex)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

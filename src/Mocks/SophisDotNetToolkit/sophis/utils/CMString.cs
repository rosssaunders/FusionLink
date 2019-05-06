//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace sophis.utils
{
    public class CMString : IDisposable
    {
        public string StringValue { get; }

        public void Dispose()
        {
        }

        public string GetString()
        {
            throw new NotImplementedException();
        }

        public static implicit operator CMString(string d)
        {
            throw new NotImplementedException();
        }
    }
}

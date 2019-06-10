//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace Sophis.Event
{
    public class SophisEventManager : IDisposable
    {
        public static SophisEventManager Instance
        {
            get;
        }

        public void Dispatch()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
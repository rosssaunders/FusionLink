//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink
{
    public class DataUpdatedFromProviderEventArgs : EventArgs
    {
        public DataUpdatedFromProviderEventArgs(TimeSpan timeTaken)
        {
            TimeTaken = timeTaken;
        }

        public TimeSpan TimeTaken { get; }
    }
}
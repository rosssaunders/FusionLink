//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink
{
    public class DataUpdatedFromProviderEventArgs : EventArgs
    {
        public DataUpdatedFromProviderEventArgs(TimeSpan timeTaken, TimeSpan overallTime)
        {
            UITimeTaken = timeTaken;
            OverallTime = overallTime;
        }

        public TimeSpan UITimeTaken { get; }

        public TimeSpan OverallTime { get; }
    }
}
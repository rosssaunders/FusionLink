//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink
{
    public class SubscriptionChangedEventArgs<T> : EventArgs
    {
        public SubscriptionChangedEventArgs(T key)
        {
            Key = key;
        }

        public T Key { get; }
    }
}

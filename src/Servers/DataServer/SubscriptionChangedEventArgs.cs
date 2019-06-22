//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

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

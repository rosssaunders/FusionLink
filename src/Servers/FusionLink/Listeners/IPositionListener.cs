//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink.Listeners
{
    public interface IPositionListener
    {
        event EventHandler<PositionChangedEventArgs> PositionChanged;
    }
}
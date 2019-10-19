//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink.Listeners
{
    public interface IPositionListener
    {
        event EventHandler<PositionChangedEventArgs> PositionChanged;
    }
}
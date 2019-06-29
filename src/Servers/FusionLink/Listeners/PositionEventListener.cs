﻿//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using sophis.portfolio;

namespace RxdSolutions.FusionLink
{
    public class PositionEventListener : CSMPositionEvent
    {
        public static event EventHandler<PositionChangedEventArgs> PositionChanged;

        public override bool HasBeenDeleted(CSMPosition position)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(position.GetIdentifier(), false));

            return base.HasBeenDeleted(position);
        }

        public override bool HasBeenModified(CSMPosition position)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(position.GetIdentifier(), false));

            return base.HasBeenModified(position);
        }

        public override void HasBeenTransferred(CSMPosition position)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(position.GetIdentifier(), false));

            base.HasBeenTransferred(position);
        }
    }
}
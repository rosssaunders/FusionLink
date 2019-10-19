//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using sophis.portfolio;
using sophis.tools;

namespace RxdSolutions.FusionLink.Listeners
{
    public class PositionActionListener : CSMPositionAction
    {
        public static event EventHandler<PositionChangedEventArgs> PositionChanged;

        public override void NotifyDeleted(CSMPosition position, CSMEventVector message)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(position.GetIdentifier(), true));

            base.NotifyDeleted(position, message);
        }

        public override void NotifyModified(CSMPosition position, CSMEventVector message)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(position.GetIdentifier(), true));

            base.NotifyModified(position, message);
        }

        public override void NotifyTransferred(CSMPosition position, CSMEventVector message)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(position.GetIdentifier(), true));

            base.NotifyTransferred(position, message);
        }
    }
}
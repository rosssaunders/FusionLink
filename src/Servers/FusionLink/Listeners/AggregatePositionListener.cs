//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink.Listeners
{
    public class AggregatePositionListener : IPositionListener
    {
        private readonly PositionActionListener _actionListener;
        private readonly PositionEventListener _eventListener;

        public event EventHandler<PositionChangedEventArgs> PositionChanged;

        public AggregatePositionListener(PositionActionListener actionListener, PositionEventListener eventListener)
        {
            _actionListener = actionListener;
            _eventListener = eventListener;

            PositionActionListener.PositionChanged += OnPositionChanged;
            PositionEventListener.PositionChanged += OnPositionChanged;
        }

        private void OnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            PositionChanged?.Invoke(this, e);
        }
    }
}
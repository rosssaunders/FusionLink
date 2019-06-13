//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink
{
    public class AggregateListener : IPortfolioListener
    {
        private readonly PortfolioActionListener _actionListener;
        private readonly PortfolioEventListener _eventListener;

        public event EventHandler<PortfolioChangedEventArgs> PortfolioChanged;

        public AggregateListener(PortfolioActionListener actionListener, PortfolioEventListener eventListener)
        {
            _actionListener = actionListener;
            _eventListener = eventListener;

            _actionListener.PortfolioChanged += OnPortfolioChanged;
            _eventListener.PortfolioChanged += OnPortfolioChanged;
        }

        private void OnPortfolioChanged(object sender, PortfolioChangedEventArgs e)
        {
            PortfolioChanged?.Invoke(this, e);
        }
    }
}
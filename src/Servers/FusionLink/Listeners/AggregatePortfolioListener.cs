//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink.Listeners
{
    public class AggregatePortfolioListener : IPortfolioListener
    {
        private readonly PortfolioActionListener _actionListener;
        private readonly PortfolioEventListener _eventListener;

        public event EventHandler<PortfolioChangedEventArgs> PortfolioChanged;

        public AggregatePortfolioListener(PortfolioActionListener actionListener, PortfolioEventListener eventListener)
        {
            _actionListener = actionListener;
            _eventListener = eventListener;

            PortfolioActionListener.PortfolioChanged += OnPortfolioChanged;
            PortfolioEventListener.PortfolioChanged += OnPortfolioChanged;
        }

        private void OnPortfolioChanged(object sender, PortfolioChangedEventArgs e)
        {
            PortfolioChanged?.Invoke(this, e);
        }
    }
}
﻿//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;

namespace RxdSolutions.FusionLink
{
    public class PortfolioCalculationEndedEventArgs : EventArgs
    {
        public int PortfolioRefreshVersion { get; }

        public PortfolioCalculationEndedEventArgs(int portfolioRefreshVersion)
        {
            PortfolioRefreshVersion = portfolioRefreshVersion;
        }
    }
}
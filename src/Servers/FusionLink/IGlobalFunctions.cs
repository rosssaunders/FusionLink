//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink
{
    public interface IGlobalFunctions
    {
        event EventHandler<PortfolioCalculationEndedEventArgs> PortfolioCalculationEnded;
    }
}
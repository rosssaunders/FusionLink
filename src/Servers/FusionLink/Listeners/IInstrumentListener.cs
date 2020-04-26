//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink.Listeners
{
    public interface IInstrumentListener
    {
        event EventHandler<InstrumentChangedEventArgs> InstrumentChanged;
    }
}
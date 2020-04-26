//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using sophis.instrument;

namespace RxdSolutions.FusionLink.Listeners
{
    public class InstrumentEventListener : CSMInstrumentEvent
    {
        public static event EventHandler<InstrumentChangedEventArgs> InstrumentChanged;

        public override void HasBeenCreated(int instrumentId)
        {
            InstrumentChanged?.Invoke(this, new InstrumentChangedEventArgs(instrumentId, true));

            base.HasBeenCreated(instrumentId);
        }

        public override void HasBeenDeleted(int instrumentId)
        {
            InstrumentChanged?.Invoke(this, new InstrumentChangedEventArgs(instrumentId, false));

            base.HasBeenDeleted(instrumentId);
        }

        public override void HasBeenModified(int instrumentId)
        {
            InstrumentChanged?.Invoke(this, new InstrumentChangedEventArgs(instrumentId, false));

            base.HasBeenModified(instrumentId);
        }
    }
}
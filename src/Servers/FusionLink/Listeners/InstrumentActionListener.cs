//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using NSREnums;
using sophis.instrument;
using sophis.portfolio;
using sophis.tools;

namespace RxdSolutions.FusionLink.Listeners
{
    public class InstrumentActionListener : CSMInstrumentAction
    {
        public static event EventHandler<InstrumentChangedEventArgs> InstrumentChanged;

        public override void NotifyCreated(CSMInstrument instrument, CSMEventVector message)
        {
            InstrumentChanged?.Invoke(this, new InstrumentChangedEventArgs(instrument.GetCode(), true));
        }

        public override void NotifyDeleted(CSMInstrument instrument, CSMEventVector message)
        {
            InstrumentChanged?.Invoke(this, new InstrumentChangedEventArgs(instrument.GetCode(), true));
        }

        public override void NotifyModified(CSMInstrument instrument, CSMEventVector message)
        {
            InstrumentChanged?.Invoke(this, new InstrumentChangedEventArgs(instrument.GetCode(), true));
        }

        public override void NotifyModified(CSMInstrument instrument, eMParameterModificationType type, CSMEventVector message)
        {
            InstrumentChanged?.Invoke(this, new InstrumentChangedEventArgs(instrument.GetCode(), true));
        }
    }
}
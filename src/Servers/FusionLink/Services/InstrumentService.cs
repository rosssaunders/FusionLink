//  Copyright (c) RXD Solutions. All rights reserved.
using sophis.inflation;
using sophis.instrument;
using sophis.utils;

namespace RxdSolutions.FusionLink.Services
{
    public class InstrumentService
    {
        public object GetValue<T>(T instrument, string property) where T : CSMInstrument
        {
            return InstrumentFields.GetValue(instrument, property);
        }
    }
}

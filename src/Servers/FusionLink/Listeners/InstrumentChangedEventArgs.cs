//  Copyright (c) RXD Solutions. All rights reserved.


namespace RxdSolutions.FusionLink.Listeners
{
    public class InstrumentChangedEventArgs
    {
        public int InstrumentId { get; }

        public bool IsLocal { get; }

        public InstrumentChangedEventArgs(int instrumentId, bool isLocal)
        {
            InstrumentId = instrumentId;
            IsLocal = isLocal;
        }
    }
}
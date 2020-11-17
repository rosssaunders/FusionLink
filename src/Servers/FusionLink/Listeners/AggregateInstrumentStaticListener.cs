//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Diagnostics;

namespace RxdSolutions.FusionLink.Listeners
{
    public class AggregateInstrumentStaticListener : IInstrumentListener
    {
        public event EventHandler<InstrumentChangedEventArgs> InstrumentChanged;

        public AggregateInstrumentStaticListener()
        {
            InstrumentActionListener.InstrumentChanged += OnInstrumentChanged;
            InstrumentEventListener.InstrumentChanged += OnInstrumentChanged;
        }

        private void OnInstrumentChanged(object sender, InstrumentChangedEventArgs e)
        {
            InstrumentChanged?.Invoke(this, e);
        }
    }
}
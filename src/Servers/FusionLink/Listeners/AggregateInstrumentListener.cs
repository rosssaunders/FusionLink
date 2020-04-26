//  Copyright (c) RXD Solutions. All rights reserved.


using System;

namespace RxdSolutions.FusionLink.Listeners
{
    public class AggregateInstrumentListener : IInstrumentListener
    {
        private readonly InstrumentActionListener _actionListener;
        private readonly InstrumentEventListener _eventListener;

        public event EventHandler<InstrumentChangedEventArgs> InstrumentChanged;

        public AggregateInstrumentListener(InstrumentActionListener actionListener, InstrumentEventListener eventListener)
        {
            _actionListener = actionListener;
            _eventListener = eventListener;

            InstrumentActionListener.InstrumentChanged += OnInstrumentChanged;
            InstrumentEventListener.InstrumentChanged += OnInstrumentChanged;
        }

        private void OnInstrumentChanged(object sender, InstrumentChangedEventArgs e)
        {
            InstrumentChanged?.Invoke(this, e);
        }
    }
}
//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using Sophis.Event;

namespace RxdSolutions.FusionLink.Listeners
{
    public class AggregateInstrumentMarketDataListener : IInstrumentListener, IDisposable
    {
        private bool _disposedValue;
        private SophisEventHandler _realtimeEventHandler;

        public event EventHandler<InstrumentChangedEventArgs> InstrumentChanged;

        public AggregateInstrumentMarketDataListener()
        {
            _realtimeEventHandler = new SophisEventHandler(ProcessRealTimeEvent);

            SophisEventManager.Instance.AddHandler(_realtimeEventHandler, Thread.MainProcess, Layer.Model, Quotation.ClassWhat);
            SophisEventManager.Instance.AddHandler(_realtimeEventHandler, Thread.MainProcess, Layer.Model, HistoricPrice.ClassWhat);
        }

        private void OnInstrumentChanged(object sender, InstrumentChangedEventArgs e)
        {
            InstrumentChanged?.Invoke(sender, e);
        }

        private void ProcessRealTimeEvent(IEvent evt, ref bool deleteEvent)
        {
            if (evt is Quotation q)
            {
                var sicovam = q.GetSico();

                if (sicovam.HasValue && sicovam.Value != 0)
                {
                    OnInstrumentChanged(this, new InstrumentChangedEventArgs(sicovam.Value, false));
                }

                return;
            }

            if (evt is HistoricPrice hp)
            {
                foreach (var hpd in hp.GetHistoricPriceData())
                {
                    OnInstrumentChanged(this, new InstrumentChangedEventArgs(hpd.Sico, false));
                }

                return;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    SophisEventManager.Instance.RemoveHandler(_realtimeEventHandler);
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
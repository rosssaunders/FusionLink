//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections;
using System.Diagnostics;
using sophis.instrument;
using sophis.misc;
using sophis.portfolio;
using sophis.utils;
using sophis.value;
using Sophis.Event;

namespace RxdSolutions.FusionLink.Listeners
{
    public class F9CalculationEndedEventArgs : EventArgs
    {
        public int FolioId { get; }

        public F9CalculationEndedEventArgs(int folioId)
        {
            FolioId = folioId;
        }
    }

    public class PortfolioCalculationEndedEventArgs : EventArgs
    {
        public int ExtractionId { get; }

        public int FolioId { get; }

        public PortfolioCalculationEndedEventArgs(int extractionId, int folioId)
        {
            ExtractionId = extractionId;
            FolioId = folioId;
        }
    }

    public class PortfolioCalculationListener
    {
        public event EventHandler<PortfolioCalculationEndedEventArgs> PortfolioCalculationEnded;
        public event EventHandler<F9CalculationEndedEventArgs> F9CalculationEnded;

        private bool _disposedValue;
        private SophisEventHandler _realtimeEventHandler;

        public PortfolioCalculationListener()
        {
            _realtimeEventHandler = new SophisEventHandler(ProcessRealTimeEvent);

            SophisEventManager.Instance.AddHandler(_realtimeEventHandler, Thread.MainProcess, Layer.Model, PortfolioF9.ClassWhat);
            SophisEventManager.Instance.AddHandler(_realtimeEventHandler, Thread.MainProcess, Layer.Model, PortfolioAnyComputationEnded.ClassWhat); 
        }

        private void OnPortfolioCalculated(object sender, PortfolioCalculationEndedEventArgs e)
        {
            PortfolioCalculationEnded?.Invoke(sender, e);
        }

        private void ProcessRealTimeEvent(IEvent evt, ref bool deleteEvent)
        {
            if (evt is PortfolioF9 f9)
            {
                var folioCode = f9.FolioCode;

                var eventArgs = new F9CalculationEndedEventArgs(folioCode);
                
                F9CalculationEnded?.Invoke(this, eventArgs);
            }
            else if(evt is PortfolioAnyComputationEnded ce)
            {
                var extractionId = ce.ExtractionId;
                var folioCode = ce.FolioCode;

                var eventArgs = new PortfolioCalculationEndedEventArgs(extractionId, folioCode);

                PortfolioCalculationEnded?.Invoke(this, eventArgs);
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
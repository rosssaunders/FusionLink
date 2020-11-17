//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using Sophis.Event;
using Thread = Sophis.Event.Thread;

namespace RxdSolutions.FusionLink.Listeners
{
    public class PreferenceChangeListener : IDisposable
    {
        private static eMAutomaticComputingType _currentPreference;
        private bool disposedValue;

        public event EventHandler AutomaticComputingChanged;

        public PreferenceChangeListener()
        {
            _currentPreference = CSMPreference.GetAutomaticComputatingType();

            var _handler = new SophisEventHandler(ProcessEvent);
            SophisEventManager.Instance.AddHandler(_handler, Thread.MainProcess, Layer.Model);
        }

        private void ProcessEvent(IEvent evt, ref bool deleteEvent)
        {
            if (evt is PreferenceUpdate pref)
            {
                if (CSMPreference.GetAutomaticComputatingType() != _currentPreference)
                {
                    _currentPreference = CSMPreference.GetAutomaticComputatingType();

                    AutomaticComputingChanged?.Invoke(new object(), new EventArgs());
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SophisEventManager.Instance.RemoveHandler(ProcessEvent);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

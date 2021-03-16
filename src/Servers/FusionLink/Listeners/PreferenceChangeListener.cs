//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RxdSolutions.FusionLink.Listeners
{
    public class PreferenceChangeListener : IDisposable
    {
        private static bool _running;
        private static Task _monitor;
        private static readonly AutoResetEvent _wait = new AutoResetEvent(true);
        private static eMAutomaticComputingType _currentPreference;
        private static Dispatcher Dispatcher;
        private bool disposedValue;

        public event EventHandler AutomaticComputingChanged;

        public void Start()
        {
            _running = true;
            Dispatcher = Dispatcher.CurrentDispatcher;
            _currentPreference = CSMPreference.GetAutomaticComputatingType();

            _monitor = Task.Run(() =>
            {
                while (_running)
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        if (CSMPreference.GetAutomaticComputatingType() != _currentPreference)
                        {
                            _currentPreference = CSMPreference.GetAutomaticComputatingType();

                            AutomaticComputingChanged?.Invoke(new object(), new EventArgs());
                        }

                    }, DispatcherPriority.ApplicationIdle);

                    _wait.WaitOne(2000);
                }
            });
        }

        public static void Stop()
        {
            _running = false;
            _wait.Set();
            _monitor.Wait();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
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
    
    /*
     * The events below are only triggered once the user uses the preferences manager to save the preference set
     * We need to react quickly to the user changing their computing preferences so the only way seems to be 
     * as above we the setting is polled on a timer.
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
    */
}

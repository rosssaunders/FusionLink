using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RxdSolutions.FusionLink.Listeners
{
    public class AutomaticComputatingPreferenceChangeListener
    {
        private static bool _running;
        private static Task _monitor;
        private static AutoResetEvent _wait = new AutoResetEvent(true);
        private static eMAutomaticComputingType _currentPreference;
        private static Dispatcher Dispatcher;

        public static event EventHandler AutomaticComputatingChanged;

        public static void Start()
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

                            AutomaticComputatingChanged?.Invoke(new object(), new EventArgs());
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
    }
}

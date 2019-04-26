using System;
using System.Configuration;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using sophis;

namespace DGP.ReportingWarehouse.Sophis
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Program p = null;

            try
            {
                p = new Program(args[0]);

                p.Run();
            }
            catch (Exception)
            {
            }
            finally
            {
                p?.Shutdown();
            }
        }

        ManualResetEvent resetEvent = new ManualResetEvent(false);

        private string _user;

        public Program(string user)
        {
            _user = user;
        }

        public void Run()
        {
            
        }

        /// <summary>
        /// Startup the Sophis API
        /// </summary>
        public void Initialize()
        {
            CSMApiHost.SetUsername(_user);

            //This is actually backed by a Lazy variable to just by calling the property
            var instance = CSMApiHost.Instance;
        }

        [HandleProcessCorruptedStateExceptions] 
        public void Shutdown()
        {
            //Sophis occassionally hangs when calling Dispose on the Api object.
            //To work around this we could just kill the process, but we'll attempt to
            //give Sophis 60 seconds to shutdown cleanly. If it does not return within
            //60 seconds, we kill the process
            Task.Run(() =>
            {
                if (resetEvent.WaitOne(TimeSpan.FromSeconds(60)))
                    return;

                //Kill this process
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            });

            try
            {
                if(CSMApiHost.IsApiInitialized)
                {
                    CSMApiHost.Instance.DisposeApi();
                    CSMApiHost.Instance.Dispose();
                }
            }
            catch (Exception)
            {
                //Sink
            }
            finally
            {
                resetEvent.Set();
            }
        }
    }
}
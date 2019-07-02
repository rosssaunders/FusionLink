using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionLink.Interface;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class AvailableConnections : IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        public event EventHandler<EventArgs> AvailableEndpointsChanged;

        public bool IsSearchingForEndPoints { get; private set; }

        private readonly List<EndpointAddress> _availableEndpoints;
        private readonly ServiceHost _announcementServiceHost;
        private readonly AnnouncementService _announcementService;

        public AvailableConnections()
        {
            _availableEndpoints = new List<EndpointAddress>();

            // Subscribe the announcement events
            _announcementService = new AnnouncementService();
            _announcementService.OnlineAnnouncementReceived += OnOnlineEvent;
            _announcementService.OfflineAnnouncementReceived += OnOfflineEvent;

            // Create ServiceHost for the AnnouncementService
            _announcementServiceHost = new ServiceHost(_announcementService);
            _announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
            _announcementServiceHost.Open();
        }

        public IReadOnlyList<EndpointAddress> AvailableEndpoints
        {
            get
            {
                lock (_availableEndpoints)
                {
                    return _availableEndpoints.ToList();
                }
            }
        }

        public Task FindAvailableServicesAsync()
        {
            IsSearchingForEndPoints = true;

            return Task.Run(() => {

                FindAvailableServices();

            });
        }

        public void FindAvailableServices()
        {
            try
            {
                IsSearchingForEndPoints = true;

                AvailableEndpointsChanged?.Invoke(this, new EventArgs());

                var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

                var findResponse = discoveryClient.Find(new FindCriteria(typeof(IDataServiceServer)) { Duration = TimeSpan.FromSeconds(3) });

                discoveryClient.Close();

                int SessionIdFromUri(EndpointDiscoveryMetadata metaData)
                {
                    if (int.TryParse(metaData.ListenUris[0].Segments[2].Replace("/", ""), out int sessionId))
                    {
                        return sessionId;
                    }
                    
                    return 0;
                }

                int ProcessIdFromUri(EndpointDiscoveryMetadata metaData)
                {
                    if (int.TryParse(metaData.ListenUris[0].Segments[3].Replace("/", ""), out int processId))
                    {
                        return processId;
                    }

                    return 0;
                }

                foreach (var endPoint in findResponse.Endpoints)
                {
                    if (string.Compare(endPoint.ListenUris[0].Host, Environment.MachineName, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        continue;
                    }

                    if (SessionIdFromUri(endPoint) != Process.GetCurrentProcess().SessionId)
                    {
                        continue;
                    }

                    //Check the process is running under the same user account
                    var processId = ProcessIdFromUri(endPoint);

                    if (processId == 0)
                        continue;

                    var remoteProcess = Process.GetProcessById(processId);
                    if (remoteProcess == null)
                        continue;

                    if(!GetProcessUser(remoteProcess).User.Value.Equals(WindowsIdentity.GetCurrent().User.Value))
                    {
                        continue;
                    }

                    var found = false;

                    foreach (var knownEndpoint in AvailableEndpoints)
                    {
                        if (knownEndpoint.Uri == endPoint.Address.Uri)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        lock (_availableEndpoints)
                        {
                            _availableEndpoints.Add(endPoint.Address);
                            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                        }
                    }
                }

                foreach (var knownEndpoint in AvailableEndpoints)
                {
                    var found = false;
                    foreach (var endPoints in findResponse.Endpoints)
                    {
                        if (knownEndpoint.Uri == endPoints.Address.Uri)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        lock (_availableEndpoints)
                        {
                            _availableEndpoints.Remove(knownEndpoint);
                            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
                        }
                    }
                }
            }
            finally
            {
                IsSearchingForEndPoints = false;

                AvailableEndpointsChanged?.Invoke(this, new EventArgs());
            }
        }

        public void Remove(EndpointAddress ea)
        {
            _availableEndpoints.Remove(ea);

            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
        }

        private void OnOfflineEvent(object sender, AnnouncementEventArgs e)
        {
            _availableEndpoints.Remove(e.EndpointDiscoveryMetadata.Address);
            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
        }

        private void OnOnlineEvent(object sender, AnnouncementEventArgs e)
        {
            _availableEndpoints.Add(e.EndpointDiscoveryMetadata.Address);
            AvailableEndpointsChanged?.Invoke(this, new EventArgs());
        }

        public EndpointAddress FindEndpoint(Uri connection)
        {
            lock (_availableEndpoints)
            {
                return _availableEndpoints.SingleOrDefault(x => x.Uri == connection);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _announcementServiceHost.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        private static WindowsIdentity GetProcessUser(Process process)
        {
            IntPtr processHandle = IntPtr.Zero;
            try
            {
                OpenProcessToken(process.Handle, 8, out processHandle);
                return new WindowsIdentity(processHandle);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    CloseHandle(processHandle);
                }
            }
        }
    }
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RxdSolutions.FusionLink.Client
{
    public class DiagnosticsViewModel : INotifyPropertyChanged
    {
        private int _portfolioSubscriptionCount;
        private int _systemSubscriptionCount;
        private int _positionSubscriptionCount;
        private int _portfolioPropertySubscriptionCount;
        private int _clientCount;
        private TimeSpan _lastTimeTaken;

        private readonly DataServer _dataServer;

        public DiagnosticsViewModel(DataServer dataServer)
        {
            _dataServer = dataServer;
            _dataServer.OnSubscriptionChanged += OnSubscriptionChanged;
            _dataServer.OnClientConnectionChanged += OnClientConnectionChanged;
            _dataServer.OnDataReceived += OnDataReceived;
            
            PortfolioSubscriptionCount = _dataServer.PortfolioValueSubscriptionCount;
            SystemSubscriptionCount = _dataServer.SystemValueCount;
            PositionSubscriptionCount = _dataServer.PositonValueSubscriptionCount;
            PortfolioPropertySubscriptionCount = _dataServer.PortfolioPropertySubscriptionCount;
            ClientCount = _dataServer.ClientCount;
            
        }

        private void OnDataReceived(object sender, DataAvailableEventArgs e)
        {
            LastTimeTaken = e.TimeTaken;
        }

        private void OnClientConnectionChanged(object sender, ClientConnectionChangedEventArgs e)
        {
            ClientCount = _dataServer.ClientCount;
        }

        private void OnSubscriptionChanged(object sender, EventArgs e)
        {
            PortfolioSubscriptionCount = _dataServer.PortfolioValueSubscriptionCount;
            SystemSubscriptionCount = _dataServer.SystemValueCount;
            PositionSubscriptionCount = _dataServer.PositonValueSubscriptionCount;
            PortfolioPropertySubscriptionCount = _dataServer.PortfolioPropertySubscriptionCount;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int ClientCount
        {
            get { return _clientCount; }
            set
            {
                _clientCount = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan LastTimeTaken
        {
            get { return _lastTimeTaken; }
            set
            {
                _lastTimeTaken = value;
                OnPropertyChanged();
            }
        }

        public string ServerUri
        {
            get { return DataServerHostFactory.GetListeningAddress().ToString(); }
        }

        public bool IsTerminalServices
        {
            get
            {
                return System.Windows.Forms.SystemInformation.TerminalServerSession;
            }
        }

        public int PortfolioSubscriptionCount
        {
            get { return _portfolioSubscriptionCount; }
            set
            {
                _portfolioSubscriptionCount = value;
                OnPropertyChanged();
            }
        }

        public int SystemSubscriptionCount
        {
            get { return _systemSubscriptionCount; }
            set
            {
                _systemSubscriptionCount = value;
                OnPropertyChanged();
            }
        }

        public int PositionSubscriptionCount
        {
            get { return _positionSubscriptionCount; }
            set
            {
                _positionSubscriptionCount = value;
                OnPropertyChanged();
            }
        }

        public int PortfolioPropertySubscriptionCount
        {
            get { return _portfolioPropertySubscriptionCount; }
            set
            {
                _portfolioPropertySubscriptionCount = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string callerMemberName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
        }
    }
}

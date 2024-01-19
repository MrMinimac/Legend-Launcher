using System.Windows.Media;

namespace LegendLauncher.ViewModel
{
    public class DescriptionViewModel : BaseViewModel
    {
        private static DescriptionViewModel _instance;
        public static DescriptionViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DescriptionViewModel();
                }
                return _instance;
            }
        }

        private string _onlinePlayers;
        public string OnlinePlayers
        {
            get { return _onlinePlayers; }
            set
            {
                if (_onlinePlayers != value)
                {
                    _onlinePlayers = value;
                    OnPropertyChanged(nameof(OnlinePlayers));
                }
            }
        }

        private string _serverStatus;
        public string ServerStatus
        {
            get { return _serverStatus; }
            set
            {
                if (_serverStatus != value)
                {
                    _serverStatus = value;
                    OnPropertyChanged(nameof(ServerStatus));
                }
            }
        }

        private Brush _serverStatusColor;
        public Brush ServerStatusColor
        {
            get { return _serverStatusColor; }
            set 
            { 
                if (value != _serverStatusColor)
                {
                    _serverStatusColor = value;
                    OnPropertyChanged(nameof(ServerStatusColor));
                }
            
            }
        }

        private bool _playersBorderVisibility;
        public bool PlayersBorderVisibility
        {
            get { return _playersBorderVisibility; }
            set
            {
                if (!_playersBorderVisibility)
                {
                    _playersBorderVisibility = value;
                    OnPropertyChanged(nameof(PlayersBorderVisibility));
                }
            }
        }
    }
}

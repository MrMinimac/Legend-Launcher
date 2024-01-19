namespace LegendLauncher.ViewModel
{
    public class LoaderBorderViewModel : BaseViewModel
    {
        private static LoaderBorderViewModel _instance;
        public static LoaderBorderViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoaderBorderViewModel();
                }
                return _instance;
            }
        }

        private bool _isPlayButtonEnabled;
        public bool IsPlayButtonEnabled
        {
            get { return _isPlayButtonEnabled; }
            set
            {
                if (_isPlayButtonEnabled != value)
                {
                    _isPlayButtonEnabled = value;
                    OnPropertyChanged(nameof(IsPlayButtonEnabled));
                }
            }
        }

        private bool _visibility;
        public bool Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged(nameof(Visibility));
                }
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        private string _procentage;
        public string Procentage
        {
            get { return _procentage; }
            set
            {
                if (_procentage != value)
                {
                    _procentage = value;
                    OnPropertyChanged(nameof(Procentage));
                }
            }
        }

    }
}

using LegendLauncher.Managers;
using LegendLauncher.View;
using LegendLauncher.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LegendLauncher.Pages
{

    public partial class HomePage : Page
    {
        InstallationManager installationManager;
        LoaderBorderViewModel lbViewModel;
        DescriptionViewModel dViewModel;
        DescriptionView descriptionView;
        AppSettingsManager appSettings;
        SettingsView settingsView;
        PlayersView playersView;

        SkinChangerView skinChangerView;

        MapView mapView;
        private bool IsServerOnline = false;

        public HomePage(Frame frame)
        {
            InitializeComponent();
            InitializeViewModel();
            LoadView();
            LoadUserSettings();
            LoadPage();
        }

        #region Initialize View Models
        private void InitializeViewModel()
        {
            lbViewModel = new LoaderBorderViewModel();
            installationManager = new InstallationManager();
            lbViewModel = LoaderBorderViewModel.Instance;
            lbViewModel.Visibility = false;
            lbViewModel.IsPlayButtonEnabled = true;
            DataContext = lbViewModel;
        }
        #endregion

        #region Load Page
        private async void LoadPage()
        {
            HomePageGrid.Opacity = 0;
            for (double i = 0; i < 1.0; i += 0.1)
            {
                HomePageGrid.Opacity = i;
                await Task.Delay(20);
            }
        }
        #endregion

        #region Load Views
        private void LoadView()
        {
            descriptionView = new DescriptionView();
            settingsView = new SettingsView();
            playersView = new PlayersView();
            mapView = new MapView();

            if (descriptionView != null)
            {
                DescriptionButton.IsChecked = true;
            }
        }
        #endregion

        #region Load User Settings
        private async void LoadUserSettings()
        {
            IsServerOnline = await ServerInfoManager.IsServerOnline();
            appSettings = AppSettingsManager.Load();
            dViewModel = DescriptionViewModel.Instance;

            PlayerName.Text = appSettings.UserName.ToUpper();

            string URL = "https://mc-heads.net/avatar/";
            BitmapImage bitmapImage;

            if (IsServerOnline)
            {
                string HeadID = DataBaseManager.GetAvatar(appSettings.Uuid);
                bitmapImage = new BitmapImage(new Uri(URL + HeadID));

                try
                {
                    PlayersButton.IsEnabled = true;
                    MapButton.IsEnabled = true;

                    #region server status
                    dViewModel.OnlinePlayers = $"{ServerInfoManager.GetOnlinePlayers()}";
                    dViewModel.PlayersBorderVisibility = true;
                    dViewModel.ServerStatus = "Включен";
                    dViewModel.ServerStatusColor = Brushes.LightGreen;
                    #endregion

                    // Отображаем отыгранное время. Один тик 20 секунд. В одной минут 1200 тиков.
                    int tickCount = DataBaseManager.GetStats(appSettings.Uuid, "PLAY_ONE_TICK");
                    PlayerTime.Text = $"{tickCount / 1200 / 60} ч. {tickCount / 1200 % 60} м.";

                    // Отображаем пройденое растояния бегом
                    int Sprint = DataBaseManager.GetStats(appSettings.Uuid, "SPRINT_ONE_CM");
                    PlayerSprint.Text = (Sprint / 100).ToString();

                    // Отображаем пройденое растояния пешком
                    int Walk = DataBaseManager.GetStats(appSettings.Uuid, "WALK_ONE_CM");
                    PlayerWalk.Text = $"{Walk / 100}";

                    // Отображаем количесво убиств
                    int MobKills = DataBaseManager.GetStats(appSettings.Uuid, "MOB_KILLS");
                    PlayerMobKills.Text = $"{MobKills}";

                    // Отображаем количество смертей
                    int Deaths = DataBaseManager.GetStats(appSettings.Uuid, "DEATHS");
                    PlayerDeaths.Text = $"{Deaths}";

                    // Отображаем кол-во нанесенного урона
                    int damageDealt = DataBaseManager.GetStats(appSettings.Uuid, "DAMAGE_DEALT");
                    DamageDealt.Text = $"{damageDealt}";

                    // Отображаем У/C
                    if (MobKills != 0 && Deaths != 0)
                    {
                        double KD = (double)MobKills / (double)Deaths;
                        PlayerKD.Text = KD.ToString("0.00");
                    }

                    // Отображаем баланс
                    string Balance = DataBaseManager.GetBalance(appSettings.Uuid);
                    PlayerBalance.Text = $"{Balance}$";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось отобразить статистику." + ex.Message);
                }
            }
            else
            {
                bitmapImage = new BitmapImage(new Uri(URL));
                PlayersButton.IsEnabled = false;
                MapButton.IsEnabled = false;
                dViewModel.ServerStatus = "Выключен";
                dViewModel.PlayersBorderVisibility = false;
                dViewModel.ServerStatusColor = Brushes.DarkRed;
            }

            UserImage.Source = bitmapImage;
        }
        #endregion

        #region Play Button Click
        private async void PlayButton_Click(Object sender, RoutedEventArgs e)
        {
            lbViewModel.IsPlayButtonEnabled = false;
            await Task.Run(() => installationManager.InstallAndRunMinecraft("1.12.2", appSettings.UserName, appSettings.RamValue));
        }
        #endregion

        #region Radio Buttons
        private async Task AnimateOpacityChange(double targetOpacity, UIElement view)
        {
            if (BorderFrame.Opacity > 0.9)
            {
                BorderFrame.Navigate(view);
                BorderFrame.Opacity = 0;
                for (double i = 0; i < 1.0; i += 0.1)
                {
                    BorderFrame.Opacity = i;
                    await Task.Delay(20);
                }
            }
            else
            {
                BorderFrame.Opacity = 1;
                BorderFrame.Navigate(view);
            }
        }

        private async void DescriptionButton_Checked(object sender, RoutedEventArgs e)
        {
            await AnimateOpacityChange(1.0, descriptionView);
        }

        private async void SettingsButton_Checked(object sender, RoutedEventArgs e)
        {
            await AnimateOpacityChange(1.0, settingsView);
        }

        private async void PlayersButton_Checked(object sender, RoutedEventArgs e)
        {
            await AnimateOpacityChange(1.0, playersView);
        }

        private async void MapButton_Checked(object sender, RoutedEventArgs e)
        {
            await AnimateOpacityChange(1.0, mapView);
        }
        #endregion

        #region Go to Skin Changer View Button
        private async void SkinChangerButton_Click(object sender, RoutedEventArgs e)
        {
            if (await ServerInfoManager.IsServerOnline())
            {
                DescriptionButton.IsChecked = false;
                PlayersButton.IsChecked = false;
                SettingsButton.IsChecked = false;

                if (skinChangerView == null)
                {
                    skinChangerView = new SkinChangerView();
                }

                await AnimateOpacityChange(1.0, skinChangerView);
            }
            else
            {
                MessageBox.Show("Сервера выключены или нет подключение к интернету", "Ошибка");
            }
        }
        #endregion

        #region Update Profile Button
        private async void UpdateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateProfileButton.IsEnabled = false;
            appSettings = AppSettingsManager.Load();
            if (appSettings.UserName != string.Empty && appSettings.UserName != null && await ServerInfoManager.IsServerOnline())
            {
                appSettings.Uuid = DataBaseManager.GetUUID(appSettings.UserName);
                appSettings.Save();
                UpdateProfileButton.IsEnabled = true;
            }
            else
            {
                UpdateProfileButton.IsEnabled = true;
            }
            LoadUserSettings();
        }
        #endregion

        #region Logout Button
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            appSettings = AppSettingsManager.Load();
            appSettings.Uuid = string.Empty;
            appSettings.Save();
            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName);
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

    }
}

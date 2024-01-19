using LegendLauncher.Managers;
using LegendLauncher.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LegendLauncher.View
{
    public partial class PlayersView : UserControl
    {
        AppSettingsManager appSettings;

        public PlayersView()
        {
            InitializeComponent();
            appSettings = new AppSettingsManager();
            Loaded += PlayersView_Loaded;
        }

        #region Show All Player
        private async Task ShowAllPlayers()
        {
            List<Player> players = await DataBaseManager.GetAllPlayers();

            players.Sort((p1, p2) =>
            {
                if (p1.IsOnline == "Green" && p2.IsOnline != "Green")
                    return -1;
                else if (p1.IsOnline != "Green" && p2.IsOnline == "Green")
                    return 1;
                else
                    return 0;
            });

            listOfPlayers.ItemsSource = players;
        }
        #endregion

        #region Players View Loaded 
        private async void PlayersView_Loaded(object sender, RoutedEventArgs e)
        {
            listOfPlayers.Visibility = Visibility.Visible;
            PlayerInfoGrid.Visibility = Visibility.Hidden;
            await ShowAllPlayers();
        }
        #endregion

        #region Text Box Changed
        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string username = NameBox.Text;
            GetInfoAboutPlayer(username);
            if (NameBox.Text != string.Empty)
            {
                ClearButton.Visibility = Visibility.Visible;
            }
            else
            {
                ClearButton.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region List View Selection Changed
        private void listOfPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOfPlayers.SelectedItem != null)
            {
                // Получаем выбранный объект Player
                Player selectedPlayer = (Player)listOfPlayers.SelectedItem;
                GetInfoAboutPlayer(selectedPlayer.Name);
                NameBox.Text = selectedPlayer.Name;

            }
        }
        #endregion

        #region Get Info About Player
        private void GetInfoAboutPlayer(string username)
        {
            string uuid = DataBaseManager.GetUUID(username);
            if (uuid != null && uuid != string.Empty)
            {
                listOfPlayers.Visibility = Visibility.Hidden;
                PlayerInfoGrid.Visibility = Visibility.Visible;
                string URL = "https://mc-heads.net/avatar/";
                string HeadID = DataBaseManager.GetAvatar(uuid);
                BitmapImage bitmapImage = new BitmapImage(new Uri(URL + HeadID));
                UserImage.Source = bitmapImage;

                // Отображаем имя
                PlayerName.Text = username.ToUpper();

                try
                {
                    // Отображаем отыгранное время. Один тик 20 секунд. В одной минут 1200 тиков.
                    int tickCount = DataBaseManager.GetStats(uuid, "PLAY_ONE_TICK");
                    PlayerTime.Text = $"{tickCount / 1200 / 60} ч. {tickCount / 1200 % 60} м.";

                    // Отображаем пройденое растояния бегом
                    int Sprint = DataBaseManager.GetStats(uuid, "SPRINT_ONE_CM");
                    PlayerSprint.Text = (Sprint / 100).ToString();

                    // Отображаем пройденое растояния пешком
                    int Walk = DataBaseManager.GetStats(uuid, "WALK_ONE_CM");
                    PlayerWalk.Text = $"{Walk / 100}";

                    // Отображаем количесво убиств
                    int MobKills = DataBaseManager.GetStats(uuid, "MOB_KILLS");
                    PlayerMobKills.Text = $"{MobKills}";

                    // Отображаем количество смертей
                    int Deaths = DataBaseManager.GetStats(uuid, "DEATHS");
                    PlayerDeaths.Text = $"{Deaths}";

                    // Отображаем кол-во нанесенного урона
                    int damageDealt = DataBaseManager.GetStats(uuid, "DAMAGE_DEALT");
                    DamageDealt.Text = $"{damageDealt}";

                    // Отображаем У/C
                    if (MobKills != 0 && Deaths != 0)
                    {
                        double KD = (double)MobKills / (double)Deaths;
                        PlayerKD.Text = KD.ToString("0.00");
                    }

                    // Отображаем баланс
                    string Balance = DataBaseManager.GetBalance(uuid);
                    PlayerBalance.Text = $"{Balance}$";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось отобразить статистику." + ex.Message);
                }
            }
            else
            {
                PlayerInfoGrid.Visibility = Visibility.Collapsed;
                listOfPlayers.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Show All Player
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            NameBox.Clear();
            ClearButton.Visibility = Visibility.Hidden;
            listOfPlayers.SelectedItems.Clear();
        }
        #endregion    
    }
}

using LegendLauncher.Managers;
using LegendLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LegendLauncher.View
{
    public partial class SkinChangerView : UserControl
    {
        AppSettingsManager appSettings = new AppSettingsManager();
        Skins selectedPlayer;

        public ObservableCollection<Skins> SkinList { get; set; }
        private string selectedPlayerName;

        public SkinChangerView()
        {
            InitializeComponent();
            Loaded += SkinChangerView_Loaded;
            SkinsScrollViewer.Visibility = Visibility.Visible;
            SkinViewer.Visibility = Visibility.Hidden;
        }

        #region View Loaded
        private async void SkinChangerView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSkinViewer();
            if (SkinList == null)
            {
                SkinList = new ObservableCollection<Skins>();
                List<Skins> skinsList = await DataBaseManager.GetSkinsUUID();
                SkinsItemsControl.ItemsSource = SkinList;
                foreach (var skin in skinsList)
                {
                    Skins newSkin = new Skins
                    {
                        Name = skin.Name,
                        Url = $"https://crafatar.com/renders/head/{skin.Uuid}",
                        Uuid = skin.Uuid,
                    };
                    await Task.Delay(100);
                    SkinList.Add(newSkin);
                }
            }
        }
        #endregion

        #region Update Skin Viewer
        private void UpdateSkinViewer()
        {
            appSettings = AppSettingsManager.Load();
            string skinUuid = DataBaseManager.GetAvatar(appSettings.Uuid);
            BitmapImage bitmapImage = new BitmapImage(new Uri($"https://crafatar.com/renders/body/{skinUuid}"));
            PlayerSkinImage.Source = bitmapImage;
        }
        #endregion

        #region On Item Click
        private void Item_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedData = ((FrameworkElement)sender).DataContext;

            if (selectedData != null)
            {
                selectedPlayer = (Skins)selectedData;
                selectedPlayerName = selectedPlayer.Name;
                NameOfSkin.Text = selectedPlayerName;
                SkinsScrollViewer.Visibility = Visibility.Hidden;
                SkinViewer.Visibility = Visibility.Visible;
                SkinImage.Source = new BitmapImage(new Uri($"https://crafatar.com/renders/body/{selectedPlayer.Uuid}"));
                Debug.WriteLine($"Selected player name: {selectedPlayerName}");
            }
        }
        #endregion

        #region Set Skin Button Click
        private void SetSkin_Click(object sender, RoutedEventArgs e)
        {
            appSettings = AppSettingsManager.Load();
            DataBaseManager.SetAvatar(appSettings.Uuid, selectedPlayer.Uuid);
            UpdateSkinViewer();
            SkinsScrollViewer.Visibility = Visibility.Visible;
            SkinViewer.Visibility = Visibility.Hidden;
            SkinsScrollViewer.PageUp();
        }
        #endregion

        #region Go To Back Button Click
        private void GoToBack_Click(object sender, RoutedEventArgs e)
        {
            SkinsScrollViewer.Visibility = Visibility.Visible;
            SkinViewer.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Copy Button Click
        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedPlayerName))
            {
                string copyText = $"/skin {selectedPlayer.Name}";
                Clipboard.SetText(copyText);
                for (double i = 0.0; i < 1.0 ; i += 0.1) 
                {
                    CopyStatus.Opacity = i;
                    await Task.Delay(10);
                }
                await Task.Delay(1000);
                for (double i = 1.0; i > 0.0; i -= 0.1)
                {
                    CopyStatus.Opacity = i;
                    await Task.Delay(10);
                }
            }
        }
        #endregion

    }
}

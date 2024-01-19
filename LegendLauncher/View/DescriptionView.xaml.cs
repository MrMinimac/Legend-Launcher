using LegendLauncher.Managers;
using LegendLauncher.Model;
using LegendLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace LegendLauncher.View
{
    public partial class DescriptionView : UserControl
    {
        DescriptionViewModel dViewModel;
        PlayersView playersView;

        public DescriptionView()
        {
            InitializeComponent();
            ShowDescription();
            dViewModel = new DescriptionViewModel();
            dViewModel = DescriptionViewModel.Instance;
            DataContext = dViewModel;
            ShowInfo();
        }

        #region Animate Description
        private async void ShowDescription()
        {
            char[] chars = "Наш лаунчер Minecraft предоставляет игрокам возможность запускать игру с  модификациями, основные из них DivineRPG, Draconic Evolution, Applied Energistics 2 и Industrial Craft 2. Эти моды обогащают геймплей, добавляя новые измерения, мощные инструменты, автоматизацию ресурсов и передовые технологии. Наш лаунчер обеспечивает удобство установки, что позволяет игрокам наслаждаться расширенным и улучшенным опытом в мире Minecraft.".ToArray();

            foreach (char c in chars)
            {
                DescriptionTextBlock.Text += c;
                await Task.Delay(5);
            }
        }
        #endregion

        #region Show Info About Server
        public async void ShowInfo()
        {
            if (await ServerInfoManager.IsServerOnline())
            {
                dViewModel.PlayersBorderVisibility = true;
                dViewModel.ServerStatus = "Включен";
                dViewModel.ServerStatusColor = Brushes.LightGreen;
                List<Player> players = await DataBaseManager.GetAllPlayers();
                List<string> playerNames = players.Select(player => player.Name).ToList();
                PlayersBorder.ToolTip = string.Join(Environment.NewLine, playerNames).ToUpper();
            }
            else
            {
                dViewModel.PlayersBorderVisibility = false;
                dViewModel.ServerStatus = "Выключен";
                dViewModel.ServerStatusColor = Brushes.DarkRed;
            }
        }
        #endregion
    }
}

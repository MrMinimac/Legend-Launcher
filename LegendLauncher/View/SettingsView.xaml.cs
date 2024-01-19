using LegendLauncher.Managers;
using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;

namespace LegendLauncher.View
{

    public partial class SettingsView : UserControl
    {
        AppSettingsManager appSettings = new AppSettingsManager();

        private bool isPerformanceCounterInitialized = false;
        private string Theme;
        private bool isDark;

        public SettingsView()
        {
            InitializeComponent();
            InitializeMaxRam();
            UpdateRamSlider();
            UpdateCheckBox();
        }

        #region Initialize Max RAM
        private void InitializeMaxRam()
        {
            try
            {
                ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(objectQuery);
                ManagementObjectCollection results = searcher.Get();

                foreach (ManagementObject result in results)
                {
                    long maxRamInBytes = Convert.ToInt64(result["TotalPhysicalMemory"]);

                    double maxRamInGigabytes = maxRamInBytes / (1024 * 1024 * 1024);
                    double maxRam = Math.Round(maxRamInGigabytes, 1);

                    Dispatcher.Invoke(() =>
                    {
                        RamSlider.Maximum = maxRam;
                    });

                    isPerformanceCounterInitialized = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing max RAM: {ex.Message}");
            }
        }
        #endregion

        #region RAM Slider Value Changed
        private void RamSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            appSettings = AppSettingsManager.Load();

            if (appSettings != null)
            {
                appSettings.RamValue = RamSlider.Value;
                appSettings.Save();
            }
        }
        #endregion

        #region Update RAM Slider
        private void UpdateRamSlider()
        {
            appSettings = AppSettingsManager.Load();

            if (appSettings != null)
            {
                RamSlider.Value = appSettings.RamValue;
            }
        }
        #endregion

        #region Update Check Box
        private void UpdateCheckBox()
        {
            if (appSettings != null)
            {
                appSettings = AppSettingsManager.Load();
                Theme = appSettings.IsTheme;

                if (Theme == "PurpleTheme")
                {
                    PurpleTheme.IsSelected = true;
                }
                if (Theme == "GreenTheme")
                {
                    GreenTheme.IsSelected = true;
                }
                if (Theme == "OrangeTheme")
                {
                    OrangeTheme.IsSelected = true;
                }
                if (Theme == "DefaultTheme")
                {
                    DefaultTheme.IsSelected = true;
                }
                if (Theme == "RedTheme")
                {
                    RedTheme.IsSelected = true;
                }
            }
        }
        #endregion

        #region Theme Selection Changed
        private void ThemeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получите выбранный элемент
            ListBox listBox = (ListBox)sender;
            ListBoxItem selectedItem = (ListBoxItem)listBox.SelectedItem;

            // Вызовите метод, связанный с выбранным элементом
            if (selectedItem != null)
            {
                // Передайте имя выбранного элемента в метод HandleSelectedItem
                ThemeSelectedItem(selectedItem.Name);
            }
        }
        #endregion

        #region Theme Selected
        private void ThemeSelectedItem(string selectedItemName)
        {
            appSettings = AppSettingsManager.Load();
            isDark = appSettings.IsDarkTheme;

            switch (selectedItemName)
            {
                case "PurpleTheme":

                    if (Theme != "PurpleTheme")
                    {
                        ThemeManager.ChangeTheme("PurpleTheme", isDark);
                        Theme = "PurpleTheme";
                    }

                    break;

                case "GreenTheme":

                    if (Theme != "GreenTheme")
                    {
                        ThemeManager.ChangeTheme("GreenTheme", isDark);
                        Theme = "GreenTheme";
                    }

                    break;

                case "OrangeTheme":

                    if (Theme != "OrangeTheme")
                    {
                        ThemeManager.ChangeTheme("OrangeTheme", isDark);
                        Theme = "OrangeTheme";
                    }

                    break;

                case "DefaultTheme":

                    if (Theme != "DefaultTheme")
                    {
                        ThemeManager.ChangeTheme("DefaultTheme", isDark);
                        Theme = "DefaultTheme";
                    }
                    break;

                case "RedTheme":


                    if (Theme != "RedTheme")
                    {
                        ThemeManager.ChangeTheme("RedTheme", isDark);
                        Theme = "RedTheme";
                    }

                    break;

                default:

                    break;
            }

            appSettings = AppSettingsManager.Load();

            if (appSettings != null)
            {
                appSettings.IsTheme = Theme;
                appSettings.Save();
                Console.WriteLine($"Сохраняем тему {Theme}");
            }
        }
        #endregion
    }
}

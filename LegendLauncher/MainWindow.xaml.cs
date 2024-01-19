using LegendLauncher.Managers;
using LegendLauncher.Pages;
using System;
using System.Windows;
using System.Windows.Input;

namespace LegendLauncher
{
    
    public partial class MainWindow : Window
    {
        AppSettingsManager appSettings = new AppSettingsManager();

        private string Theme;
        private bool isDark;

        private static MainWindow _instance;
        public static MainWindow Instance
        {
            get { return _instance; }
        }

        public MainWindow()
        {
            InitializeComponent();

            UpdateManager updateManager = new UpdateManager(this);
            appSettings = AppSettingsManager.Load();

            if (appSettings.Uuid != null && appSettings.Uuid != string.Empty) 
            {
                HomePage homePage = new HomePage(MainFraim);
                MainFraim.Navigate(homePage);
            }
            else
            {
                LoginPage loginPage = new LoginPage(MainFraim);
                MainFraim.Navigate(loginPage);
            }

            _instance = this;
        }

        #region Load Theme Settings
        private void LoadThemeSettings()
        {
            try
            {
                appSettings = AppSettingsManager.Load();
                Theme = appSettings.IsTheme;
                isDark = appSettings.IsDarkTheme;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке настроек: {ex.Message}");
            }
        }
        #endregion

        #region Dark/Light Theme Button
        public void DarkLightThemeButton(object sender, RoutedEventArgs e)
        {
            LoadThemeSettings();
            ThemeManager.ChangeTheme(Theme, !isDark);
            appSettings.IsDarkTheme = !isDark;
            appSettings.Save();
        }
        #endregion

        #region Window Manager
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void MinWindow()
        {
            var page = Window.GetWindow(this);
            page.WindowState = WindowState.Minimized;
        }

        public void NormalizeWindow()
        {
            var page = Window.GetWindow(this);
            page.WindowState = WindowState.Normal;
        }
        #endregion
    }
}

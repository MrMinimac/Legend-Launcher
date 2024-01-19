using LegendLauncher.Managers;
using System;
using System.Threading;
using System.Windows;

namespace LegendLauncher
{
    public partial class App : Application
    {
        AppSettingsManager appSettings = new AppSettingsManager();
        public static readonly string AppIconUrl = "pack://application:,,,/Legend.ico";
        private const string MutexName = "LegendLauncherMutex";
        private string Theme;
        private bool isDark;

        #region Change Theme
        public void ChangeTheme(Uri uri)
        {
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
        #endregion

        #region On Startup
        protected override void OnStartup(StartupEventArgs e)
        {
            Mutex mutex = new Mutex(true, MutexName, out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Лаунчер уже запущен. Проверьте значок в трее");
                Environment.Exit(0);
            }
            base.OnStartup(e);
            LoadTheme();
        }
        #endregion

        #region Load Theme
        private void LoadTheme()
        {
            LoadThemeSettings();
            if (Theme != null && Theme != string.Empty)
            {
                try
                {
                    ThemeManager.ChangeTheme(Theme, isDark);
                }
                catch
                {
                    ThemeManager.ChangeTheme("DefaultTheme", isDark);
                    appSettings = AppSettingsManager.Load();
                    appSettings.IsTheme = "DefaultTheme";
                    appSettings.Save();
                }
            }
        }
        #endregion

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
    }
}
